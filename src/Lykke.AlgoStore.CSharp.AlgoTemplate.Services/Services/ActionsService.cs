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
        private ITradingService tradingService;
        private Action<Exception, string> onErrorHandler;
        private IStatisticsService statisticsService;

        /// <summary>
        /// Initializes new instance of ActionsService
        /// </summary>
        /// <param name="tradingService">The <see cref="ITradingService"/> 
        /// implementation for providing the trading capabilities</param>
        /// <param name="statisticsService">The <see cref="IStatisticsService"/> 
        /// implementation for providing the statics capabilities</param>
        /// <param name="onErrorHandler">A handler to be executed upon error</param>
        public ActionsService(ITradingService tradingService,
            IStatisticsService statisticsService,
            Action<Exception, string> onErrorHandler)
        {
            this.tradingService = tradingService;
            this.statisticsService = statisticsService;
            this.onErrorHandler = onErrorHandler;
        }

        public double BuyStraight(double volume)
        {
            try
            {
                var price = this.tradingService.BuyStraight(volume);

                if (price.Result > 0)
                {
                    this.statisticsService.OnAction(true, volume, price);
                }

                return price.Result;
            }
            catch (Exception e)
            {
                onErrorHandler.Invoke(e, "Meaningful message for the user");
                // If we can not return. re-throw.
                throw;
            }
        }

        public double BuyReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public double SellReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public double SellStraight(double volume)
        {
            throw new NotImplementedException();
        }
    }
}
