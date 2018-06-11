using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System;

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

        public double Buy(IAlgoQuote quoteData, double volume)
        {
            try
            {
                var request = GetTradeRequest(volume, quoteData.Price, quoteData.DateReceived);
                var result = _tradingService.Buy(request);

                HandleResponse(result.Result, true, request);

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a buy order.");
                // If we can not return. re-throw.
                throw;
            }
        }

        public double Buy(IAlgoCandle candleData, double volume)
        {
            try
            {
                var request = GetTradeRequest(volume, candleData.Close, candleData.DateTime);
                var result = _tradingService.Buy(request);
                HandleResponse(result.Result, true, request);

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a buy order.");
                // If we can not return. re-throw.
                throw;
            }
        }

        public void Log(string message)
        {
            var instanceId = _algoSettingsService.GetInstanceId();

            _logService.Write(instanceId, message);
        }

        public double Sell(IAlgoQuote quoteData, double volume)
        {
            try
            {
                var request = GetTradeRequest(volume, quoteData.Price, quoteData.DateReceived);
                var result = _tradingService.Sell(request);
                HandleResponse(result.Result, false, request);

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a sell order.");
                // If we can not return. re-throw.
                throw;
            }
        }

        public double Sell(IAlgoCandle candleData, double volume)
        {
            try
            {
                var request = GetTradeRequest(volume, candleData.LastTradePrice, candleData.DateTime);
                var result = _tradingService.Sell(request);
                HandleResponse(result.Result, false, request);

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a sell order.");
                // If we can not return. re-throw.
                throw;
            }
        }

        private void HandleResponse(ResponseModel<double> result, bool isBuy, ITradeRequest tradeRequest)
        {
            string action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                Log($"There was a problem placing a {action} order. Error: {result.Error.Message} is buying - {isBuy} ");
            }

            if (result.Result > 0)
            {
                _statisticsService.OnAction(isBuy, tradeRequest.Volume, result.Result);

                var dateTime = _algoSettingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test
                    ? tradeRequest.Date
                    : DateTime.UtcNow;

                Log($"A {action} order successful: {tradeRequest.Volume} {_algoSettingsService.GetTradedAssetId()} - price {result.Result} at {dateTime.ToDefaultDateTimeFormat()}");
            }
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
