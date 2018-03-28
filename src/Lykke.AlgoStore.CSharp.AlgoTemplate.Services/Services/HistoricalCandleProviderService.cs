using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class HistoricalCandleProviderService : ICandleProviderService
    {
        private class CandleSourceData
        {
            public IEnumerator<Candle> CandleSource { get; set; }
            public bool IsSourceDone { get; set; }
        }

        private class SubscriptionData
        {
            public string AssetPair { get; set; }
            public CandleTimeInterval TimeInterval { get; set; }
            public Action<Candle> Callback { get; set; }
        }

        private readonly IHistoryDataService _historyDataService;
        private readonly DateTime _startDate;

        private readonly List<SubscriptionData> _subscriptions = new List<SubscriptionData>();
        private readonly Dictionary<CandleTimeInterval, Dictionary<string, CandleSourceData>> _providers =
            new Dictionary<CandleTimeInterval, Dictionary<string, CandleSourceData>>();
        private readonly object _sync = new object();

        private CancellationTokenSource _cts;
        private Thread _workerThread;

        private bool _isStopped;

        public HistoricalCandleProviderService(IHistoryDataService historyDataService, DateTime startDate)
        {
            _historyDataService = historyDataService;
            _startDate = startDate;
        }

        public void Subscribe(string assetPair, CandleTimeInterval timeInterval, Action<Candle> callback)
        {
            if (assetPair == null)
                throw new ArgumentNullException(nameof(assetPair));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (!_providers.ContainsKey(timeInterval))
                _providers.Add(timeInterval, new Dictionary<string, CandleSourceData>());

            if (!_providers[timeInterval].ContainsKey(assetPair))
            {
                _providers[timeInterval].Add(assetPair, new CandleSourceData
                {
                    CandleSource = _historyDataService.GetHistory(new Core.Domain.CandlesHistoryRequest
                    {
                        AssetPair = assetPair,
                        Interval = timeInterval,
                        From = _startDate
                    }).GetEnumerator()
                });
            }

            _subscriptions.Add(new SubscriptionData
            {
                AssetPair = assetPair,
                Callback = callback,
                TimeInterval = timeInterval
            });
        }

        public void Start()
        {
            if (_isStopped)
                throw new NotSupportedException("Cannot restart the provider after it has been stopped");

            if (_workerThread != null)
                throw new InvalidOperationException("Provider is already started");

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

                var intervalsToUpdate = GetIntervalsForUpdate(currentDate);

                intervalsToUpdate = intervalsToUpdate.Where(c => _providers.ContainsKey(c) && 
                                                                 _providers[c].Values.Any(csd => csd.CandleSource.MoveNext()))
                                                     .ToHashSet();

                if (intervalsToUpdate.Count == 0) break;

                foreach (var subscription in _subscriptions)
                {
                    if (!intervalsToUpdate.Contains(subscription.TimeInterval))
                        continue;

                    subscription.Callback(_providers[subscription.TimeInterval][subscription.AssetPair].CandleSource.Current);
                }
            }
            while (!cancellationToken.IsCancellationRequested);

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
