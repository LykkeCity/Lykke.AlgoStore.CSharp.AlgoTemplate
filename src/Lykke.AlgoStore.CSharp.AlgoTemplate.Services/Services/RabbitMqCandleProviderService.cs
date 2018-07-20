using Common.Log;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.Job.CandlesProducer.Contract;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CandleTimeInterval = Lykke.Job.CandlesProducer.Contract.CandleTimeInterval;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class RabbitMqCandleProviderService : ICandleProviderService
    {
        private class CallbackInfo
        {
            public Action<Candle> Callback { get; set; }
            public DateTime StartFrom { get; set; }
            public DateTime EndOn { get; set; }
        }

        private class SubscriptionData
        {
            public List<CallbackInfo> Callbacks { get; } = new List<CallbackInfo>();
            public Thread CandleGenerator { get; set; }
            public Candle PrevCandle { get; set; }
            public Candle CurrentCandle { get; set; }
            public CandleTimeInterval TimeInterval { get; set; }

            public object Sync { get; } = new object();
        }

        private readonly IReloadingManager<BaseRabbitMqSubscriptionSettings> _settings;
        private readonly ILog _log;
        private readonly IAlgoSettingsService _algoSettingsService;

        private RabbitMqSubscriber<CandlesUpdatedEvent> _subscriber;
        private bool _disposed;

        // Fast subscription lookup by asset pair and candle time interval
        private readonly Dictionary<string, Dictionary<CandleTimeInterval, SubscriptionData>> _subscriptions = new Dictionary<string, Dictionary<CandleTimeInterval, SubscriptionData>>();

        public RabbitMqCandleProviderService(IReloadingManager<BaseRabbitMqSubscriptionSettings> settings, ILog log, IAlgoSettingsService algoSettingsService)
        {
            _settings = settings;
            _log = log;
            _algoSettingsService = algoSettingsService;
        }

        public void Subscribe(CandleServiceRequest serviceRequest, Action<Candle> callback)
        {
            if (serviceRequest == null)
                throw new ArgumentNullException(nameof(serviceRequest));

            var assetPair = serviceRequest.AssetPair;
            var timeInterval = serviceRequest.CandleInterval;
            
            if (!_subscriptions.ContainsKey(assetPair))
                _subscriptions.Add(assetPair, new Dictionary<CandleTimeInterval, SubscriptionData>());

            var contractTimeInterval = (CandleTimeInterval)timeInterval;

            if (!_subscriptions[assetPair].ContainsKey(contractTimeInterval))
                _subscriptions[assetPair].Add(contractTimeInterval, CreateSubscriptionData(contractTimeInterval));

            _subscriptions[assetPair][contractTimeInterval].Callbacks.Add(new CallbackInfo
            {
                StartFrom = serviceRequest.StartFrom,
                EndOn = serviceRequest.EndOn,
                Callback = callback
            });
        }

        public void SetPrevCandleFromHistory(string assetPair, Models.Enumerators.CandleTimeInterval timeInterval, Candle candle)
        {
            if (assetPair == null)
                throw new ArgumentNullException(nameof(assetPair));
            if (candle == null)
                throw new ArgumentNullException(nameof(candle));

            if (!_subscriptions.ContainsKey(assetPair))
                return;

            var contractTimeInterval = (CandleTimeInterval)timeInterval;

            if (!_subscriptions[assetPair].ContainsKey(contractTimeInterval))
                return;

            var subscriptionData = _subscriptions[assetPair][contractTimeInterval];

            lock (subscriptionData.Sync)
            {
                subscriptionData.PrevCandle = candle;
            }
        }

        public async Task Initialize()
        {
            await _settings.Reload();

            var currentSettings = _settings.CurrentValue;

            var rabbitSettings = RabbitMqSubscriptionSettings.CreateForSubscriber(currentSettings.ConnectionString, currentSettings.NamespaceOfSourceEndpoint,
                                                                                  currentSettings.NameOfSourceEndpoint, currentSettings.NamespaceOfEndpoint,
                                                                                  $"{currentSettings.NameOfEndpoint}-{_algoSettingsService.GetInstanceId()}");
            rabbitSettings.DeadLetterExchangeName = null;

            if (currentSettings.IsDurable)
                rabbitSettings.MakeDurable();

            _subscriber = new RabbitMqSubscriber<CandlesUpdatedEvent>(rabbitSettings,
                new ResilientErrorHandlingStrategy(_log, rabbitSettings,
                        retryTimeout: TimeSpan.FromMilliseconds(currentSettings.ReconnectionDelayInMSec),
                        retryNum: currentSettings.ReconnectionsCountToAlarm,
                        next: new DeadQueueErrorHandlingStrategy(_log, rabbitSettings)))
                .SetLogger(_log)
                .SetMessageDeserializer(new MessagePackMessageDeserializer<CandlesUpdatedEvent>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnCandleReceived);
        }

        public void Start()
        {
            if (_disposed)
                throw new InvalidOperationException("Restarting the Rabbit MQ provider is not allowed. Create a new one instead");

            _subscriber.Start();
        }

        public void Stop()
        {
            // Due to the difficulty of restarting the provider after it's stopped, considering all the threads
            // that have to be reset, it's better to dispose it here too
            _subscriber.Stop();
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var pair in _subscriptions.Values)
            {
                foreach (var data in pair.Values)
                {
                    // Interrupt threads to wake them from potential sleep states and exit
                    data.CandleGenerator.Interrupt();
                    data.CandleGenerator.Join();
                }
            }
            _subscriptions.Clear();
            _subscriber?.Dispose();
            _subscriber = null;
            _disposed = true;
        }

        private Task OnCandleReceived(CandlesUpdatedEvent candles)
        {
            var filteredCandles = candles.Candles.Where(c => c.PriceType == CandlePriceType.Mid &&
                                                             _subscriptions.ContainsKey(c.AssetPairId) &&
                                                             _subscriptions[c.AssetPairId].ContainsKey(c.TimeInterval));

            foreach (var candle in filteredCandles)
            {
                var algoCandle = new Candle
                {
                    Open = candle.Open,
                    Close = candle.Close,
                    Low = candle.Low,
                    High = candle.High,
                    DateTime = candle.CandleTimestamp,
                    TradingVolume = candle.TradingVolume
                };

                var subscriptionData = _subscriptions[candle.AssetPairId][candle.TimeInterval];

                lock (subscriptionData.Sync)
                {
                    // Discard old updates
                    if (subscriptionData.PrevCandle != null && candle.CandleTimestamp <= subscriptionData.PrevCandle.DateTime)
                        continue;

                    subscriptionData.CurrentCandle = algoCandle;
                }
            }

            return Task.CompletedTask;
        }

        private SubscriptionData CreateSubscriptionData(CandleTimeInterval timeInterval)
        {
            var data = new SubscriptionData();

            var generator = new Thread(GeneratorThread);
            generator.Priority = ThreadPriority.AboveNormal;

            data.TimeInterval = timeInterval;
            data.CandleGenerator = generator;

            generator.Start(data);

            return data;
        }

        private void GeneratorThread(object data)
        {
            var subscriptionData = (SubscriptionData)data;
            var algoStoreInterval = (Models.Enumerators.CandleTimeInterval)subscriptionData.TimeInterval;

            var now = DateTime.UtcNow;
            DateTime nextCandleTime;

            // Figure out when the next candle is
            switch (subscriptionData.TimeInterval)
            {
                case CandleTimeInterval.Sec:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(1);
                    break;
                case CandleTimeInterval.Minute:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
                    break;
                case CandleTimeInterval.Min5:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute / 5 * 5, 0).AddMinutes(5);
                    break;
                case CandleTimeInterval.Min15:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute / 15 * 15, 0).AddMinutes(15);
                    break;
                case CandleTimeInterval.Min30:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute / 30 * 30, 0).AddMinutes(30);
                    break;
                case CandleTimeInterval.Hour:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
                    break;
                case CandleTimeInterval.Hour4:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour / 4 * 4, 0, 0).AddHours(4);
                    break;
                case CandleTimeInterval.Hour6:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour / 6 * 6, 0, 0).AddHours(6);
                    break;
                case CandleTimeInterval.Hour12:
                    nextCandleTime = new DateTime(now.Year, now.Month, now.Day, now.Hour / 12 * 12, 0, 0).AddHours(12);
                    break;
                case CandleTimeInterval.Day:
                    nextCandleTime = now.Date.AddDays(1);
                    break;
                case CandleTimeInterval.Week:
                    var diff = (7 + (now.Date.DayOfWeek - DayOfWeek.Monday)) % 7;
                    nextCandleTime = now.Date.AddDays(-1 * diff).AddDays(7);
                    break;
                case CandleTimeInterval.Month:
                    nextCandleTime = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionData.TimeInterval));
            }

            // Use a 50ms delay to allow all quotes to come
            nextCandleTime = nextCandleTime.AddMilliseconds(50);

            while (true)
            {
                var timeSpan = nextCandleTime - DateTime.UtcNow;
                try
                {
                    Thread.Sleep(timeSpan);
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }

                Candle currentCandle = null;
                CallbackInfo[] callbacks = null;

                lock (subscriptionData.Sync)
                {
                    if (subscriptionData.CurrentCandle == null)
                    {
                        if (subscriptionData.PrevCandle == null)
                        {
                            // No way to create an empty candle when there's no previous candle
                            nextCandleTime = nextCandleTime.AddSeconds((int)subscriptionData.TimeInterval);
                            continue;
                        }

                        var candle = new Candle();

                        candle.Open = candle.Close = candle.Low = candle.High = candle.LastTradePrice = subscriptionData.PrevCandle.Close;
                        candle.DateTime = algoStoreInterval.DecrementTimestamp(nextCandleTime.AddMilliseconds(-50));

                        subscriptionData.CurrentCandle = candle;
                    }

                    currentCandle = subscriptionData.CurrentCandle;
                    callbacks = subscriptionData.Callbacks.ToArray();

                    subscriptionData.PrevCandle = subscriptionData.CurrentCandle;
                    subscriptionData.CurrentCandle = null;
                }

                // To prevent any possible deadlocks, run callbacks outside of lock with a copy of the callback list
                foreach (var callbackInfo in callbacks)
                {
                    if (callbackInfo.StartFrom > currentCandle.DateTime || callbackInfo.EndOn < currentCandle.DateTime)
                        continue;

                    callbackInfo.Callback(currentCandle);
                }

                nextCandleTime = algoStoreInterval.IncrementTimestamp(nextCandleTime);
            }
        }
    }
}
