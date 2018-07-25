using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class HistoricalCandleProviderService : ICandleProviderService
    {
        private class CandleSourceData
        {
            public IEnumerator<Candle> CandleSource { get; set; }
            public DateTime From { get; set; }
        }

        private class SubscriptionData
        {
            public CandleTimeInterval TimeInterval { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public Action<Candle> Callback { get; set; }
            public IEnumerator<Candle> CandleSource { get; set; }
        }

        private readonly IHistoryDataService _historyDataService;
        private readonly IUserLogService _userLogService;
        private readonly IAlgoSettingsService _algoSettingsService;

        private readonly List<SubscriptionData> _subscriptions = new List<SubscriptionData>();

        private CancellationTokenSource _cts;
        private Thread _workerThread;

        private DateTime _startDate = DateTime.MaxValue;
        private DateTime _endDate = DateTime.MinValue;

        private bool _isStopped;

        public HistoricalCandleProviderService(IHistoryDataService historyDataService, IUserLogService userLogService,
            IAlgoSettingsService algoSettingsService)
        {
            _historyDataService = historyDataService;
            _userLogService = userLogService;
            _algoSettingsService = algoSettingsService;
        }

        public void Subscribe(CandleServiceRequest serviceRequest, Action<Candle> callback)
        {
            if (serviceRequest == null)
                throw new ArgumentNullException(nameof(serviceRequest));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (_workerThread != null)
                throw new InvalidOperationException("Cannot subscribe after service has been started");

            var subscriptionData = new SubscriptionData
            {
                Callback = callback,
                TimeInterval = serviceRequest.CandleInterval,
                StartDate = serviceRequest.StartFrom.ToUniversalTime(),
                EndDate = serviceRequest.EndOn.ToUniversalTime()
            };

            subscriptionData.CandleSource = _historyDataService.GetHistory(new Core.Domain.CandlesHistoryRequest
            {
                AssetPair = serviceRequest.AssetPair,
                Interval = serviceRequest.CandleInterval,
                AuthToken = serviceRequest.AuthToken,
                IndicatorName = serviceRequest.RequestId,
                From = subscriptionData.StartDate,
                To = subscriptionData.EndDate
            }).GetEnumerator();

            if (_startDate > subscriptionData.StartDate)
                _startDate = subscriptionData.StartDate;

            if (_endDate < subscriptionData.EndDate)
                _endDate = subscriptionData.EndDate;

            _subscriptions.Add(subscriptionData);
        }

        public void Start()
        {
            if (_isStopped)
                throw new NotSupportedException("Cannot restart the provider after it has been stopped");

            if (_workerThread != null)
                throw new InvalidOperationException("Provider is already started");

            if (_subscriptions.Count == 0)
                throw new InvalidOperationException("Cannot start provider with no subscriptions");

            _cts = new CancellationTokenSource();
            _workerThread = new Thread(FillLoop);
            _workerThread.Start(_cts.Token);
        }

        public void Stop()
        {
            if (_workerThread == null)
                throw new InvalidOperationException("Provider is already stopped");

            _cts.Cancel();
            _workerThread.Join();
            _workerThread = null;
            _isStopped = true;
        }

        public void Dispose()
        {
            if (_workerThread != null)
                Stop();
        }

        public void SetPrevCandleFromHistory(string assetPair, CandleTimeInterval timeInterval, Candle candle)
        {
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        private void FillLoop(object cancellationTokenObj)
        {
            var cancellationToken = (CancellationToken)cancellationTokenObj;
            DateTime currentDate = _startDate.AddSeconds(-1);

            do
            {
                currentDate = currentDate.AddSeconds(1);

                if (currentDate > DateTime.UtcNow || currentDate > _endDate)
                {
                    _userLogService.Enqueue(_algoSettingsService.GetInstanceId(),
                        "Current date period is reached, execution of back-test historical data is stopped.");
                    break;
                }

                var intervalsToUpdate = GetIntervalsForUpdate(currentDate);

                foreach(var subscription in _subscriptions)
                {
                    if (!intervalsToUpdate.Contains(subscription.TimeInterval)) continue;

                    if (subscription.StartDate > currentDate || subscription.EndDate < currentDate) continue;

                    // Wait forever for the history service to come back online - we're running in backtest
                    // so we wouldn't want to interrupt the algo in the middle of its operation
                    if (!subscription.CandleSource.MoveNextWithRetry(int.MaxValue).GetAwaiter().GetResult())
                        continue;

                    subscription.Callback(subscription.CandleSource.Current);
                }
            }
            while (!cancellationToken.IsCancellationRequested);

            //Use thread sleep in order pod not to be restarted, 
            //later when stopping of an algo is implemented, we should remove this thread.
            Thread.Sleep(Timeout.Infinite);

            // TODO: Shutdown algo here
        }

        private HashSet<CandleTimeInterval> GetIntervalsForUpdate(DateTime currentDate)
        {
            var hashset = new HashSet<CandleTimeInterval>();

            hashset.Add(CandleTimeInterval.Sec);

            if (currentDate.Second != 0) return hashset;

            hashset.Add(CandleTimeInterval.Minute);

            if ((currentDate.Minute % 5) != 0) return hashset;

            hashset.Add(CandleTimeInterval.Min5);

            if ((currentDate.Minute % 15) != 0) return hashset;

            hashset.Add(CandleTimeInterval.Min15);

            if ((currentDate.Minute % 30) != 0) return hashset;

            hashset.Add(CandleTimeInterval.Min30);

            if (currentDate.Minute != 0) return hashset;

            hashset.Add(CandleTimeInterval.Hour);

            if ((currentDate.Hour % 4) == 0)
                hashset.Add(CandleTimeInterval.Hour4);

            if ((currentDate.Hour % 6) == 0)
                hashset.Add(CandleTimeInterval.Hour6);

            if ((currentDate.Hour % 12) != 0) return hashset;

            hashset.Add(CandleTimeInterval.Hour12);

            if (currentDate.Hour != 0) return hashset;

            hashset.Add(CandleTimeInterval.Day);

            if (((currentDate.DayOfYear - 1) % 7) == 0)
                hashset.Add(CandleTimeInterval.Week);

            if (currentDate.Day == 1)
                hashset.Add(CandleTimeInterval.Month);

            return hashset;
        }
    }
}
