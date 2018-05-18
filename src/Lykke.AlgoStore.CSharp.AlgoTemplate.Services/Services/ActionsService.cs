using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;

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

        public double Buy(double volume)
        {
            try
            {
                var result = _tradingService.Buy(volume);

                HandleResponse(result.Result, true, volume);

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
                var result = _tradingService.Buy(volume, candleData);

                HandleResponse(result.Result, true, volume);

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

        public double Sell(double volume)
        {
            try
            {
                var result = _tradingService.Sell(volume);

                HandleResponse(result.Result, false, volume);

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
                var result = _tradingService.Sell(volume, candleData);

                HandleResponse(result.Result, false, volume);

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a sell order.");
                // If we can not return. re-throw.
                throw;
            }
        }

        private void HandleResponse(ResponseModel<double> result, bool isBuy, double volume)
        {
            string action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                Log($"There was a problem placing a {action} order. Error: {result.Error.Message} is buying - {isBuy} ");
            }

            if (result.Result > 0)
            {
                _statisticsService.OnAction(isBuy, volume, result.Result);
                Log($"A {action} order successful: {volume} {_algoSettingsService.GetTradedAsset()} - price {result.Result} at {DateTime.UtcNow}");
            }
        }
    }
}
