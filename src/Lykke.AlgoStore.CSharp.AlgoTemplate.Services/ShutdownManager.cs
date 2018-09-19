using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services
{
    // NOTE: Sometimes, shutdown process should be expressed explicitly. 
    // If this is your case, use this class to manage shutdown.
    // For example, sometimes some state should be saved only after all incoming message processing and 
    // all periodical handler was stopped, and so on.
    
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IUserLogService _userLogService;
        private readonly IEventCollector _eventCollector;
        private readonly IMarketOrderManager _marketOrderManager;
        private readonly ILimitOrderManager _limitOrderManager;

        public ShutdownManager(
            ILog log,
            IUserLogService userLogService,
            IEventCollector eventCollector,
            IMarketOrderManager marketOrderManager,
            ILimitOrderManager limitOrderManager)
        {
            _log = log;
            _userLogService = userLogService;
            _eventCollector = eventCollector;
            _marketOrderManager = marketOrderManager;
            _limitOrderManager = limitOrderManager;
        }

        public async Task StopAsync()
        {
            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "ShutdownManager start");

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "Disposing the market order manager...");

            _marketOrderManager.Dispose();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "Flushing and disposing the event collector...");

            _eventCollector.Dispose();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "Flushing and disposing the user log service...");

            _userLogService.Dispose();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "Flushing and disposing the limit order manager...");

            _limitOrderManager.Dispose();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync),
                "ShutdownManager complete");

           
        }
    }
}
