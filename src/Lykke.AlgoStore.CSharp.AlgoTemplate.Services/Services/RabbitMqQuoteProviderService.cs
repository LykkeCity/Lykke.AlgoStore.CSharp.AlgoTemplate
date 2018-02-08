using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.Job.QuotesProducer.Contract;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class RabbitMqQuoteProviderService : IQuoteProviderService
    {
        private readonly IReloadingManager<QuoteRabbitMqSubscriptionSettings> _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<QuoteMessage> _subscriber;

        private readonly object _subsciptionLock = new object();
        private List<Func<IAlgoQuote, Task>> _subscriptions = new List<Func<IAlgoQuote, Task>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqQuoteProviderService" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="log">The log.</param>
        public RabbitMqQuoteProviderService(IReloadingManager<QuoteRabbitMqSubscriptionSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="RabbitMqQuoteProviderService"/> class.
        /// </summary>
        ~RabbitMqQuoteProviderService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            await _settings.Reload();
            lock (_subsciptionLock)
            {
                _subscriptions = new List<Func<IAlgoQuote, Task>>();
            }

            var settings = _settings.CurrentValue.ToRabbitMqSettings();
            _subscriber = new RabbitMqSubscriber<QuoteMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        retryNum: 10,
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<QuoteMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(OnQuoteInternal)
                .CreateDefaultBinding()
                .SetLogger(_log);
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Subscribe(Func<IAlgoQuote, Task> action)
        {
            lock (_subsciptionLock)
            {
                _subscriptions.Add(action);
            }
        }

        /// <summary>
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            _subscriber.Start();
        }

        /// <summary>
        /// Handles quote from RabbitMq.
        /// </summary>
        /// <param name="quoteMessage">The quote message.</param>
        /// <returns></returns>
        private Task OnQuoteInternal(QuoteMessage quoteMessage)
        {
            if (quoteMessage.AssetPair != _settings.CurrentValue.SubscriptionAsset)
                return Task.CompletedTask;

            lock (_subsciptionLock)
            {
                Task[] tasks = new Task[_subscriptions.Count];

                var quote = new AlgoQuote
                {
                    IsBuy = quoteMessage.IsBuy,
                    IsOnline = true, // For use if has delay
                    Price = quoteMessage.Price,
                    Timestamp = quoteMessage.Timestamp,
                    DateReceived = DateTime.UtcNow
                };

                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = _subscriptions[i](quote);
                }

                Task.WaitAll(tasks);
                return Task.CompletedTask;
            }
        }

        #region IStopable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_subscriber != null)
            {
                _subscriber.Dispose();
                _subscriber = null;
            }
        }

        public void Stop()
        {
            if (_subscriber != null)
                _subscriber.Stop();
        }
        #endregion
    }
}
