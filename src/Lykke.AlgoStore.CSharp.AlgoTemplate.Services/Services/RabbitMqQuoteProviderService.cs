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

            // Look at https://github.com/LykkeCity/Lykke.RabbitMqDotNetBroker
            // https://github.com/LykkeCity/Lykke.RabbitMqDotNetBroker/blob/master/src/Lykke.RabbitMqBroker/Subscriber/RabbitMqSubscriber.cs
            // In general ResilientErrorHandlingStrategy will retry invoke OnQuoteInternal.GetAwaiter().GetResult()
            // specified times on specified intervals and then will continue with DeadQueueErrorHandlingStrategy
            // which will reject message
            // BUT THESE ErrorHandlingStrategy are executed AFTER message is received - They handle just OnQuoteInternal exceptions, but not reconnect
            // They try to reconnect silenlty specified in RabbitMqSubscriptionSettings times on specified intervals and on error just log fatal error
            // not so sure if this is acceptable because default values are 20 times on 3 secs = 1 min ?!?
            // please look at implementation again - but this is what i see in ReadThread
            // the only way that i see is my initial idea to wrap ILog and handle this Fatal error
            // and then on quote received to clear error
            // Based on this i think that it is not a good idea to have many subscibers - 
            // what should this service do if just one of subscibers throws ?!? - accept or reject message
            // I think we should delegate this on upper level - so just one subscriber
            // Think about it :-)
            _subscriber = new RabbitMqSubscriber<QuoteMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10), // this is just for ResilientErrorHandlingStrategy
                        retryNum: 10, // this is just for ResilientErrorHandlingStrategy - after all retries will invoke DeadQueueErrorHandlingStrategy to reject message
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

            // i think that here should NOT use reference swap
            // now we will also block next quote - this way will try to make it serial
            lock (_subsciptionLock)
            {
                var tasks = new List<Task>(_subscriptions.Count);

                var quote = new AlgoQuote
                {
                    IsBuy = quoteMessage.IsBuy,
                    IsOnline = true, // For use if has delay
                    Price = quoteMessage.Price,
                    Timestamp = quoteMessage.Timestamp,
                    DateReceived = DateTime.UtcNow
                };

                foreach (Func<IAlgoQuote, Task> subscription in _subscriptions)
                {
                    try
                    {
                        var task = subscription(quote);
                        tasks.Add(task);
                    }
                    catch (Exception ex)
                    {
                        // WHAT TO DO ?!? Now i accept message
                        _log.WriteErrorAsync(string.Empty, string.Empty, ex).Wait();
                    }
                }

                Task.WaitAll(tasks.ToArray());
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
