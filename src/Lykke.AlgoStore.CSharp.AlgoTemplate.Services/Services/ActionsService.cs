using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IActions"/> implementation
    /// </summary>
    public class ActionsService : IActions
    {
        private ITradingService tradingService;
        private Action<Exception, string> onErrorHandler;
        private IStatisticsService statisticsService;
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
            this.tradingService = tradingService;
            this.statisticsService = statisticsService;
            _algoSettingsService = algoSettingsService;
            this.onErrorHandler = onErrorHandler;          
        }

        public double Buy(double volume)
        {
            try
            {
                var result = this.tradingService.BuyStraight(volume);

                if (result.Result.Error != null)
                {
                    Console.WriteLine($"There was a problem placing a buy order. Error: {result.Result.Error.Message}");
                }

                if (result.Result.Result > 0)
                {
                    this.statisticsService.OnAction(true, volume, result.Result.Result);
                    Console.WriteLine($"Buying {volume} {_algoSettingsService.GetAlgoInstanceTradedAsset()} - price {result.Result.Result} at {DateTime.UtcNow}");
                }

                return result.Result.Result;
            }
            catch (Exception e)
            {
                onErrorHandler.Invoke(e, "There was a problem placing a buy order.");
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
                var result = this.tradingService.SellStraight(volume);

                if (result.Result.Error != null)
                {
                    Console.WriteLine($"There was a problem placing a sell order. Error: {result.Result.Error.Message}");
                }
                
                if (result.Result.Result > 0)
                {
                    this.statisticsService.OnAction(false, volume, result.Result.Result);
                    Console.WriteLine($"Selling {volume} {_algoSettingsService.GetAlgoInstanceTradedAsset()} - price {result.Result.Result} at {DateTime.UtcNow}");
                }

                return result.Result.Result;
            }
            catch (Exception e)
            {
                onErrorHandler.Invoke(e, "There was a problem placing a sell order.");
                // If we can not return. re-throw.
                throw;
            }
        }
    }
}
