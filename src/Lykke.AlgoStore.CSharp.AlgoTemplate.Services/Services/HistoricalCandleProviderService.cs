using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;

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
            public string AssetPair { get; set; }
            public CandleTimeInterval TimeInterval { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public Action<Candle> Callback { get; set; }
        }

        private readonly IHistoryDataService _historyDataService;
        private readonly IUserLogService _userLogService;
        private readonly IAlgoSettingsService _algoSettingsService;

        private readonly List<SubscriptionData> _subscriptions = new List<SubscriptionData>();
        private readonly Dictionary<CandleTimeInterval, Dictionary<string, CandleSourceData>> _providers =
            new Dictionary<CandleTimeInterval, Dictionary<string, CandleSourceData>>();
        private readonly object _sync = new object();

        private CancellationTokenSource _cts;
        private Thread _workerThread;

        private DateTime _startDate = DateTime.MaxValue;

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

            var assetPair = serviceRequest.AssetPair;
            var timeInterval = serviceRequest.CandleInterval;

            if (!_providers.ContainsKey(timeInterval))
                _providers.Add(timeInterval, new Dictionary<string, CandleSourceData>());

            _subscriptions.Add(new SubscriptionData
            {
                AssetPair = assetPair,
                Callback = callback,
                TimeInterval = timeInterval,
                StartDate = serviceRequest.StartFrom,
                EndDate = serviceRequest.EndOn
            });

            if (!_providers[timeInterval].ContainsKey(assetPair))
                _providers[timeInterval].Add(assetPair, null); // We set this later

            var fromDate = _subscriptions.Where(s => s.AssetPair == assetPair && s.TimeInterval == timeInterval)
                                         .Min(s => s.StartDate);

            if (fromDate < _startDate)
                _startDate = fromDate;

            _providers[timeInterval][assetPair] = new CandleSourceData
            {
                From = fromDate,
                CandleSource = _historyDataService.GetHistory(new Core.Domain.CandlesHistoryRequest
                {
                    AssetPair = assetPair,
                    Interval = timeInterval,
                    From = fromDate,
                    To = serviceRequest.EndOn
                }).GetEnumerator()
            };
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

                if (currentDate > DateTime.UtcNow)
                {
                    _userLogService.Write(_algoSettingsService.GetInstanceId(), "Current date period is reached, execution of back-test historiacl data is stopped.");
                    break;
                }

                var intervalsToUpdate = GetIntervalsForUpdate(currentDate);

                intervalsToUpdate = intervalsToUpdate
                    .Where(c => _providers.ContainsKey(c) &&
                                _providers[c].Values.Any(csd => (csd.CandleSource.Current == null ||
                                                                 csd.CandleSource.Current.DateTime < currentDate) &&
                                                                csd.CandleSource.MoveNext())).ToHashSet();

                foreach (var subscription in _subscriptions)
                {
                    if (!intervalsToUpdate.Contains(subscription.TimeInterval) ||
                        subscription.StartDate > currentDate ||
                        subscription.EndDate < currentDate)
                        continue;

                    var candle = _providers[subscription.TimeInterval][subscription.AssetPair].CandleSource.Current;

                    // If the candle is not for today - move on
                    // This can happen when the history service is first fetching the candles and
                    // the first one ends up some time after the current date
                    if (candle != null && candle.DateTime != currentDate) continue;

                    subscription.Callback(candle);
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
