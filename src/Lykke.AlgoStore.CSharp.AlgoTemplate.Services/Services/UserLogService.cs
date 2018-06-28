using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IUserLogService"/> implementation
    /// </summary>
    public class UserLogService : IUserLogService, IDisposable
    {
        [NotNull] private readonly ILoggingClient _userLogClient;
        [NotNull] private readonly BatchBlock<UserLogRequest> _batchBlock;
        [NotNull] private readonly ActionBlock<UserLogRequest[]> _actionBlock;

        private readonly TimeSpan _maxBatchLifetime;
        private DateTime _currentBatchExpirationMoment;
        private readonly Timer _timer;

        private readonly IAlgoSettingsService _algoSettingsService;
        private string _authToken;

        public UserLogService([NotNull] ILoggingClient userLogClient,
            TimeSpan maxBatchLifetime,
            int batchSizeThreshold,
            IAlgoSettingsService algoSettingsService)
        {
            if (maxBatchLifetime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchLifetime), maxBatchLifetime,
                    "Should be positive time span");
            }

            if (batchSizeThreshold < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSizeThreshold), batchSizeThreshold,
                    "Should be positive number");
            }

            _maxBatchLifetime = maxBatchLifetime;
            _userLogClient = userLogClient;
            _algoSettingsService = algoSettingsService;
            _algoSettingsService.Initialize();

            _batchBlock = new BatchBlock<UserLogRequest>(batchSizeThreshold);

            _actionBlock = new ActionBlock<UserLogRequest[]>(x => PersistUserLogs(x),
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 2});

            _batchBlock.LinkTo(_actionBlock);

            SetAuthToken();

            ExtendBatchExpiration();

            //Initiate timer after 1s (1000ms) with a period of 50ms
            _timer = new Timer(HandleTimerTriggered, null, 1000, 50);
        }

        private void SetAuthToken()
        {
            _authToken = _algoSettingsService.GetAuthToken();
        }

        public void Enqueue(UserLogRequest userLog)
        {
            _batchBlock.Post(userLog);
        }

        public void Enqueue(string instanceId, string message)
        {
            _batchBlock.Post(new UserLogRequest
            {
                InstanceId = instanceId,
                Date = DateTime.UtcNow,
                Message = message
            });
        }

        private Task PersistUserLogs(UserLogRequest[] userLogRequests)
        {
            return _userLogClient.WriteAsync(userLogRequests, _authToken);
        }

        private void HandleTimerTriggered(object state)
        {
            if (DateTime.UtcNow > _currentBatchExpirationMoment)
            {
                _batchBlock.TriggerBatch();

                ExtendBatchExpiration();
            }
        }

        private void ExtendBatchExpiration()
        {
            _currentBatchExpirationMoment = DateTime.UtcNow + _maxBatchLifetime;
        }

        public void Dispose()
        {
            _batchBlock.Complete();
            _batchBlock.Completion.ConfigureAwait(false).GetAwaiter().GetResult();

            _actionBlock.Complete();
            _actionBlock.Completion.ConfigureAwait(false).GetAwaiter().GetResult();

            _timer.Dispose();
        }
    }
}
