using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IActions"/> implementation
    /// </summary>
    public class ActionsService : IActions
    {
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IUserLogService _logService;

        /// <summary>
        /// Initializes new instance of ActionsService
        /// </summary>
        /// <param name="logService">The <see cref="IUserLogService"/>
        /// implementation for providing log capabilities</param>
        /// <param name="algoSettingsService">The <see cref="IAlgoSettingsService"/>
        /// implementation for providing the algo settings</param>
        public ActionsService(
            IUserLogService logService,
            IAlgoSettingsService algoSettingsService)
        {
            _logService = logService;
            _algoSettingsService = algoSettingsService;
        }

        public void Log(string message)
        {
            var instanceId = _algoSettingsService.GetInstanceId();

            _logService.Enqueue(instanceId, message);
        }
    }
}
