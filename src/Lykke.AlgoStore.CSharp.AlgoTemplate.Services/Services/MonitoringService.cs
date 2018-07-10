using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.Job.Stopping.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly IAlgoSettingsService _settingsService;
        private readonly IAlgoInstanceStoppingClient _stoppingClient;
        private readonly TimeSpan _timeout;

        public MonitoringService(
            IAlgoSettingsService settingsService,
            IAlgoInstanceStoppingClient stoppingClient,
            TimeSpan timeout)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _stoppingClient = stoppingClient ?? throw new ArgumentNullException(nameof(stoppingClient));
            _timeout = timeout;
        }

        public CancellationTokenSource StartAlgoEvent()
        {
            var cts = new CancellationTokenSource();

            var delayTask = Task.Delay(_timeout, cts.Token);

            delayTask.ContinueWith(async t =>
            {
                if (t.IsCanceled)
                    return;

                await _stoppingClient.DeleteAlgoInstanceAsync(_settingsService.GetInstanceId(), _settingsService.GetAuthToken());
            });

            return cts;
        }
    }
}
