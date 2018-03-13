using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IActions"/> implementation
    /// </summary>
    public class ActionsService : IActions
    {
        private readonly IAlgoSettingsService _settingsService;
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
        /// implementation for providing the statics capabilities</param>
        /// <param name="logService">The <see cref="IUserLogService"/>
        /// implementation for providing log capabilities</param>
        /// <param name="settingsService">The <see cref="IAlgoSettingsService"/>
        /// implementation for providing settings service capabilities</param>
        /// <param name="onErrorHandler">A handler to be executed upon error</param>
        public ActionsService(ITradingService tradingService,
            IStatisticsService statisticsService,
            IUserLogService logService,
            IAlgoSettingsService settingsService,
            Action<Exception, string> onErrorHandler)
        {
            _tradingService = tradingService;
            _statisticsService = statisticsService;
            _onErrorHandler = onErrorHandler;
            _logService = logService;
            _settingsService = settingsService;
        }

        public double Buy(double volume)
        {
            try
            {
                var price = _tradingService.BuyStraight(volume);

                if (price.Result > 0)
                {
                    _statisticsService.OnAction(true, volume, price.Result);
                }

                return price.Result;
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

            var instanceId = _settingsService.GetInstanceId();

            _logService.Write(instanceId, message);
        }

        public double Sell(double volume)
        {
            try
            {
                var price = _tradingService.SellStraight(volume);

                if (price.Result > 0)
                {
                    _statisticsService.OnAction(true, volume, price.Result);
                }

                return price.Result;
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
