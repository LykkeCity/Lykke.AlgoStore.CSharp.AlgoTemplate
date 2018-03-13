using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IActions"/> implementation
    /// </summary>
    public class ActionsService : IActions
    {
        private readonly ITradingService _tradingService;
        private readonly Action<Exception, string> _onErrorHandler;
        private readonly IStatisticsService _statisticsService;
        private readonly IAlgoSettingsService _algoSettingsService;

        /// <summary>
        /// Initializes new instance of ActionsService
        /// </summary>
        /// <param name="tradingService">The <see cref="ITradingService"/> 
        /// implementation for providing the trading capabilities</param>
        /// <param name="statisticsService">The <see cref="IStatisticsService"/> 
        /// implementation for providing the statics capabilities</param>
        /// <param name="algoSettingsService">The <see cref="IAlgoSettingsService"/>
        /// implementation for providing the algo settings</param>
        /// <param name="onErrorHandler">A handler to be executed upon error</param>
        public ActionsService(ITradingService tradingService,
            IStatisticsService statisticsService,
            IAlgoSettingsService algoSettingsService,
            Action<Exception, string> onErrorHandler)
        {
            _tradingService = tradingService;
            _statisticsService = statisticsService;
            _algoSettingsService = algoSettingsService;
            _onErrorHandler = onErrorHandler;          
        }

        public double Buy(double volume)
        {
            try
            {
                var result = _tradingService.BuyStraight(volume);

                if (result.Result.Error != null)
                {
                    Log($"There was a problem placing a buy order. Error: {result.Result.Error.Message}");
                }

                if (result.Result.Result > 0)
                {
                    _statisticsService.OnAction(true, volume, result.Result.Result);
                    Log($"Buy order successful: {volume} {_algoSettingsService.GetTradedAsset()} - price {result.Result.Result} at {DateTime.UtcNow}");
                }

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
            Console.WriteLine(message);
        }

        public double Sell(double volume)
        {
            try
            {
                var result = _tradingService.SellStraight(volume);

                if (result.Result.Error != null)
                {
                    Log($"There was a problem placing a sell order. Error: {result.Result.Error.Message}");
                }
                
                if (result.Result.Result > 0)
                {
                    _statisticsService.OnAction(false, volume, result.Result.Result);
                    Log($"Sell order successful: {volume} {_algoSettingsService.GetTradedAsset()} - price {result.Result.Result} at {DateTime.UtcNow}");
                }

                return result.Result.Result;
            }
            catch (Exception e)
            {
                _onErrorHandler.Invoke(e, "There was a problem placing a sell order.");
                // If we can not return. re-throw.
                throw;
            }
        }
    }
}
