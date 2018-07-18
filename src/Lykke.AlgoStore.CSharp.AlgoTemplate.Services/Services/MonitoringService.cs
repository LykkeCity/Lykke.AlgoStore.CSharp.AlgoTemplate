using Common.Log;
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
        private readonly IUserLogService _userLogService;
        private readonly ILog _log;
        private readonly TimeSpan _timeout;

        public MonitoringService(
            IAlgoSettingsService settingsService,
            IAlgoInstanceStoppingClient stoppingClient,
            IUserLogService userLogService,
            ILog log,
            TimeSpan timeout)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _stoppingClient = stoppingClient ?? throw new ArgumentNullException(nameof(stoppingClient));
            _userLogService = userLogService ?? throw new ArgumentNullException(nameof(userLogService));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _timeout = timeout;
        }

        public CancellationTokenSource StartAlgoEvent(string shutdownReason)
        {
            var cts = new CancellationTokenSource();

            var delayTask = Task.Delay(_timeout, cts.Token);

            delayTask.ContinueWith(async t =>
            {
                if (t.IsCanceled)
                    return;

                _userLogService.Enqueue(_settingsService.GetInstanceId(), shutdownReason);
                await _log.WriteWarningAsync(nameof(MonitoringService), nameof(StartAlgoEvent),
                            $"Algo instance event timed out and is being shut down with the following reason: {shutdownReason}");

                // Try three times if an error occurs
                for (var i = 0; i < 3; i++)
                {
                    try
                    {
                        await _stoppingClient.DeleteAlgoInstanceAsync(_settingsService.GetInstanceId(), _settingsService.GetAuthToken());
                        break;
                    }
                    catch(Exception e)
                    {
                        await _log.WriteWarningAsync(nameof(MonitoringService), nameof(StartAlgoEvent),
                            $"There was an error stopping the algo instance: {e.ToString()}");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
            });

            return cts;
        }
    }
}
