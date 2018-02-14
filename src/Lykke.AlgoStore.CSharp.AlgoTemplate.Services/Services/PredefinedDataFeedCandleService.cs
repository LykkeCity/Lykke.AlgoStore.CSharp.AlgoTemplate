using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="ICandlesService"/> implementation feeding with predefined
    /// (hard coded) candles
    /// </summary>
    public class PredefinedDataFeedCandleService : ICandlesService
    {
        private class Subscription
        {
            public IList<CandleServiceRequest> Requests { get; set; }
            public Action<IList<MultipleCandlesResponse>> InitialDataConsumer { get; set; }
            public Action<IList<SingleCandleResponse>> CandleUpdateConsumer { get; set; }
        }

        private readonly IHistoryDataService _history;
        private readonly Random _random = new Random();

        private readonly IAsyncExecutor _asyncExecutor;
        private bool _isGenerating;
        private IList<Subscription> _subscriptions = new List<Subscription>();
        private bool _isProducing = false;

        public PredefinedDataFeedCandleService(IHistoryDataService history, IAsyncExecutor asyncExecutor)
        {
            _history = history;
            _asyncExecutor = asyncExecutor;
        }

        public void Subscribe(IList<CandleServiceRequest> candleServiceRequests,
            Action<IList<MultipleCandlesResponse>> initalDataConsumer,
            Action<IList<SingleCandleResponse>> candleUpdateConsumer)
        {
            lock (_subscriptions)
            {
                var subscription = new Subscription()
                {
                    Requests = candleServiceRequests,
                    InitialDataConsumer = initalDataConsumer,
                    CandleUpdateConsumer = candleUpdateConsumer
                };

                _subscriptions.Add(subscription);

                if (_isProducing)
                {
                    ProduceHistoryData(subscription);
                }
            }

        }

        public void StartProducing()
        {
            lock (_subscriptions)
            {
                _isProducing = true;
                foreach (var subscription in _subscriptions)
                {
                    ProduceHistoryData(subscription);
                }
            }

            StartGeneratingRandomCandles();
        }

        private void ProduceHistoryData(Subscription subscription)
        {
            IList<MultipleCandlesResponse> initialDataResponses = new List<MultipleCandlesResponse>();
            foreach (var request in subscription.Requests)
            {
                var historyRequest = new CandlesHistoryRequest()
                {
                    AssetPair = request.AssetPair,
                    Interval = request.CandleInterval,
                    From = request.StartFrom
                };
                var historyData = _history.GetHistory(historyRequest);
                initialDataResponses.Add(new MultipleCandlesResponse()
                {
                    Candles = historyData,
                    RequestId = request.RequestId
                });
            }
            subscription.InitialDataConsumer(initialDataResponses);
        }

        public void StopProducing()
        {
            _isGenerating = false;
        }

        /// <summary>
        /// Start generating random candles
        /// </summary>
        private Task StartGeneratingRandomCandles()
        {
            _isGenerating = true;
            return _asyncExecutor.ExecuteAsync(() =>
            {
                while (_isGenerating)
                {
                    GenerateRandomQuoteUpdate();
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// Generate a single update to the subscribers
        /// </summary>
        public void GenerateRandomQuoteUpdate()
        {
            lock (_subscriptions)
            {
                foreach (var subscription in _subscriptions)
                {
                    IList<SingleCandleResponse> singleCandleResponse = new List<SingleCandleResponse>();
                    foreach (var request in subscription.Requests)
                    {
                        var response = new SingleCandleResponse()
                        {
                            RequestId = request.RequestId,
                            Candle = GenerateRandomCandle()
                        };
                        singleCandleResponse.Add(response);
                    }

                    subscription.CandleUpdateConsumer(singleCandleResponse);
                }
            }
        }

        /// <summary>
        /// Generates a random <see cref="Candle"/>
        /// </summary>
        /// <returns></returns>
        private Candle GenerateRandomCandle()
        {
            var historyData = _history.GetHistory(new CandlesHistoryRequest());
            var randomIndexToRepeat = _random.Next(0, historyData.Count);

            // Repeat the history with a Random index
            return historyData[randomIndexToRepeat];
        }

    }
}
