using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IQuoteProviderService"/> implementation generating a random quotes.
    /// </summary>
    public class RandomDataQuoteProviderService : IQuoteProviderService
    {
        private const int MIN_DELAY_BETWEEN_TICKS_IN_MILIS = 500;
        private const int MAX_DELAY_BETWEEN_TICKS_IN_MILIS = 2000;

        private const double INITIAL_PRICE = 100.00;
        private const int MAX_PRICE_CHANGE_ABS = 3;

        private Random _random = new Random();
        private readonly IList<Func<IAlgoQuote, Task>> subscribers = new List<Func<IAlgoQuote, Task>>();
        private readonly IAsyncExecutor asyncExecutor;

        private volatile bool _isGenerating;
        private double _lastPrice;

        /// <summary>
        /// Initializes new instance of <see cref="RandomDataQuoteProviderService"/>
        /// </summary>
        public RandomDataQuoteProviderService() : this(new TaskAsyncExecutor())
        { }

        /// <summary>
        /// Initializes new instance of <see cref="RandomDataQuoteProviderService"/>
        /// </summary>
        /// <param name="asyncExecutor"><see cref="IAsyncExecutor"/> 
        /// for running executing the quotes subscriptions</param>
        public RandomDataQuoteProviderService(IAsyncExecutor asyncExecutor)
        {
            this.asyncExecutor = asyncExecutor;
        }

        /// <summary>
        /// Initializes the quote provider
        /// </summary>
        public Task Initialize()
        {
            // Reinitialize the random
            _random = new Random();
            _lastPrice = INITIAL_PRICE;
            return StartGenerating();
        }

        /// <summary>
        /// Add a subscriber for a quote
        /// </summary>
        /// <param name="action"></param>
        public void Subscribe(Func<IAlgoQuote, Task> action)
        {
            this.subscribers.Add(action);
        }

        /// <summary>
        /// Start generating quotes
        /// </summary>
        private Task StartGenerating()
        {
            this._isGenerating = true;
            return asyncExecutor.ExecuteAsync(() =>
            {
                while (_isGenerating)
                {
                    GenerateTick();
                    var intevalBetweenTicksInMilis = _random.Next(MIN_DELAY_BETWEEN_TICKS_IN_MILIS, MAX_DELAY_BETWEEN_TICKS_IN_MILIS);
                    Thread.Sleep(intevalBetweenTicksInMilis);
                }
            });
        }

        /// <summary>
        /// Generate a single quote
        /// </summary>
        public void GenerateTick()
        {
            foreach (var subscriber in this.subscribers)
            {
                asyncExecutor.ExecuteAsync(subscriber, GenerateRandomQuote());
            }
        }

        /// <summary>
        /// Generate all pending quotes and stops generation of more qoutes.
        /// </summary>
        public void StopGenerating()
        {
            this._isGenerating = false;
        }

        /// <summary>
        /// Generates a <see cref="IAlgoQuote"/> populated with random values
        /// </summary>
        /// <returns></returns>
        private IAlgoQuote GenerateRandomQuote()
        {
            var result = new AlgoQuote();
            double priceChange = _random.Next(0, MAX_PRICE_CHANGE_ABS - 1);
            priceChange += _random.NextDouble();
            if (_random.GenerateRandomBoolean(0.51))
            {
                result.Price = _lastPrice + priceChange;
            }
            else
            {
                result.Price = _lastPrice - priceChange;
            }

            //result.Price = 
            var probabilityToBeBuy = 0.60;
            result.IsBuy = RandomBoolean(probabilityToBeBuy);
            result.IsOnline = true;
            result.Timestamp = DateTime.UtcNow;

            // Safe a random old (non-trade safe) price to be used for the next quote
            _lastPrice = result.Price;
            return result;
        }

        // TODO: Move the RandomExtensions once rebase
        private bool RandomBoolean(double probabilityToBeTrue)
        {
            return this._random.NextDouble() < probabilityToBeTrue;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
