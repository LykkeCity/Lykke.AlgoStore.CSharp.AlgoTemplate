using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IUserLogService"/> implementation
    /// </summary>
    public class UserLogService : IUserLogService
    {
        [NotNull] private readonly ILoggingClient _userLogClient;
        [NotNull] private readonly BatchSubmitter<UserLogRequest> _batchSubmitter;

        private readonly IAlgoSettingsService _algoSettingsService;
        private string _authToken;

        public UserLogService([NotNull] ILoggingClient userLogClient,
            TimeSpan maxBatchLifetime,
            int batchSizeThreshold,
            IAlgoSettingsService algoSettingsService)
        {
            _userLogClient = userLogClient;
            _algoSettingsService = algoSettingsService;

            SetAuthToken();

            _batchSubmitter = new BatchSubmitter<UserLogRequest>(
                maxBatchLifetime, batchSizeThreshold, PersistUserLogs);
        }

        private void SetAuthToken()
        {
            _authToken = _algoSettingsService.GetAuthToken();
        }

        public void Enqueue(UserLogRequest userLog)
        {
            _batchSubmitter.Enqueue(userLog);
        }

        public void Enqueue(string instanceId, string message)
        {
            _batchSubmitter.Enqueue(new UserLogRequest
            {
                InstanceId = instanceId,
                Date = DateTime.UtcNow,
                Message = message
            });
        }

        private async Task PersistUserLogs(UserLogRequest[] userLogRequests)
        {
            await _userLogClient.WriteAsync(userLogRequests, _authToken);
        }
    }
}
