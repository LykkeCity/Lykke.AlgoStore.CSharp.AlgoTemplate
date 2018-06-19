using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels.ErrorCodes;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IActions"/> implementation
    /// </summary>
    public class ActionsService : ICandleActions, IQuoteActions
    {
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IUserLogService _logService;
        private readonly ITradingService _tradingService;
        private readonly Action<Exception, string> _onErrorHandler;
        private readonly IStatisticsService _statisticsService;

        /// <summary>
        /// Initializes new instance of ActionsService
        /// </summary>
        /// <param name="tradingService">The <see cref="ITradingService"/> 
        /// implementation for providing the trading capabilities</param>
        /// <param name="statisticsService">The <see cref="IStatisticsService"/> 
        /// implementation for providing the statistics capabilities</param>
        /// <param name="logService">The <see cref="IUserLogService"/>
        /// implementation for providing log capabilities</param>
        /// <param name="algoSettingsService">The <see cref="IAlgoSettingsService"/>
        /// implementation for providing the algo settings</param>
        /// <param name="onErrorHandler">A handler to be executed upon error</param>
        public ActionsService(ITradingService tradingService,
            IStatisticsService statisticsService,
            IUserLogService logService,
            IAlgoSettingsService algoSettingsService,
            Action<Exception, string> onErrorHandler)
        {
            _tradingService = tradingService;
            _statisticsService = statisticsService;
            _logService = logService;
            _algoSettingsService = algoSettingsService;
            _onErrorHandler = onErrorHandler;
        }

        private TradeResponse ExecuteTradeRequest(Func<ITradeRequest, TradeResponse > tradeRequest, TradeRequest request)
        {
            try
            {
                return tradeRequest(request);
            }
            catch (AggregateException ex)
            {
                var error = ex.Flatten().InnerException.Message;
                Log($"There was a problem placing your order: {request}. Error: {error}. ");

                foreach (var inner in ex.Flatten().InnerExceptions)
                {
                    if (inner is System.Net.Sockets.SocketException || inner is System.IO.IOException)
                    {
                        return TradeResponse.CreateFail(TradeErrorCode.NetworkError);
                    }
                    if (inner is TaskCanceledException || inner is OperationCanceledException)
                    {
                        return TradeResponse.CreateFail(TradeErrorCode.RequestTimeout);
                    }
                }

                return TradeResponse.CreateFail(TradeErrorCode.Runtime);
            }
            catch (Exception e)
            {
                Log($"There was a problem placing your order: {request}. Error: {e.Message}. ");
                return TradeResponse.CreateFail(TradeErrorCode.Runtime);
            }
        }

        public TradeResponse Buy(IAlgoQuote quoteData, double volume)
        {
            return ExecuteTradeRequest( (tradeRequest) =>
            {
                var result = _tradingService.Buy(tradeRequest).WithCancellation();
                var response = HandleResponse(result.Result, true, tradeRequest);

                return response;
            }, GetTradeRequest(volume, quoteData.Price, quoteData.DateReceived));
        }

        public TradeResponse Buy(IAlgoCandle candleData, double volume)
        {
            return ExecuteTradeRequest((tradeRequest) =>
            {
                var result = _tradingService.Buy(tradeRequest).WithCancellation();
                var response = HandleResponse(result.Result, true, tradeRequest);

                return response;
            }, GetTradeRequest(volume, candleData.Close, candleData.DateTime));
        }

        public TradeResponse Sell(IAlgoQuote quoteData, double volume)
        {
            return ExecuteTradeRequest((tradeRequest)=>
            {
                var result = _tradingService.Sell(tradeRequest).WithCancellation();
                var response = HandleResponse(result.Result, false, tradeRequest);

                return response;
            }, GetTradeRequest(volume, quoteData.Price, quoteData.DateReceived));

        }

        public TradeResponse Sell(IAlgoCandle candleData, double volume)
        {
            return ExecuteTradeRequest((tradeRequest) =>
            {
                var result = _tradingService.Sell(tradeRequest).WithCancellation();
                var response = HandleResponse(result.Result, false, tradeRequest);
                return response;
            }, GetTradeRequest(volume, candleData.Close, candleData.DateTime));
        }

        public void Log(string message)
        {
            var instanceId = _algoSettingsService.GetInstanceId();

            _logService.Write(instanceId, message);
        }

        private TradeResponse HandleResponse(ResponseModel<double> result, bool isBuy, ITradeRequest tradeRequest)
        {
            string action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                Log($"There was a problem placing a {action} order. Error: {result.Error.Message} is buying - {isBuy} ");
                return TradeResponse.CreateFail(result.Error.Code.ToTradeErrorCode(), result.Error.Message);
            }

            if (result.Result > 0)
            {
                _statisticsService.OnAction(isBuy, tradeRequest.Volume, result.Result);

                var dateTime = _algoSettingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test
                    ? tradeRequest.Date
                    : DateTime.UtcNow;

                Log($"A {action} order successful: {tradeRequest.Volume} {_algoSettingsService.GetTradedAssetId()} - price {result.Result} at {dateTime.ToDefaultDateTimeFormat()}");

                return TradeResponse.CreateOk(result.Result);
            }

            return TradeResponse.CreateFail(TradeErrorCode.Runtime, "Unexpected (empty) response.");
        }

        private TradeRequest GetTradeRequest(double volume, double price, DateTime date)
        {
            return new TradeRequest()
            {
                Volume = volume,
                Price = price,
                Date = date
            };
        }
    }
}
