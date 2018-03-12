using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class CandlesService : ICandlesService
    {
        private readonly ICandleProviderService _candleProvider;
        private readonly IHistoryDataService _historyService;
        
        private bool _isProducing;
        private readonly List<CandleServiceRequest> _candleRequests = new List<CandleServiceRequest>();
        private Action<IList<MultipleCandlesResponse>> _initialDataConsumer;

        private bool _isHistoryDone;
        private readonly Queue<SingleCandleResponse> _responseQueue = new Queue<SingleCandleResponse>();
        private readonly object _queueSync = new object();

        public CandlesService(ICandleProviderService candleProvider, IHistoryDataService historyService)
        {
            _candleProvider = candleProvider;
            _historyService = historyService;
        }

        public void StartProducing()
        {
            if (_initialDataConsumer == null)
                throw new NotSupportedException("Cannot start producing with no subscriptions");

            if (_isProducing)
                throw new InvalidOperationException("The candle service has already started producing");

            _isHistoryDone = false;

            _candleProvider.Initialize().Wait();
            _candleProvider.Start();
            _isProducing = true;

            var resultList = new List<MultipleCandlesResponse>();

            foreach(var request in _candleRequests)
            {
                var historyRequest = new CandlesHistoryRequest
                {
                    AssetPair = request.AssetPair,
                    From = request.StartFrom,
                    Interval = request.CandleInterval
                };

                var result = _historyService.GetHistory(historyRequest);
                resultList.Add(new MultipleCandlesResponse
                {
                    RequestId = request.RequestId,
                    Candles = result
                });
            }

            FillFromHistory(resultList, _candleRequests);
            _initialDataConsumer(resultList);
        }

        public void StopProducing()
        {
            _candleProvider.Stop();
            _isProducing = false;
        }

        public void Subscribe(IList<CandleServiceRequest> candleServiceRequests, Action<IList<MultipleCandlesResponse>> initialDataConsumer, Action<IList<SingleCandleResponse>> candleUpdateConsumer)
        {
            if (_initialDataConsumer != null)
                throw new NotSupportedException("Multiple subscriptions are not yet supported");

            if (_isProducing)
                throw new InvalidOperationException("Subscription is only allowed when the service hasn't started producing yet");

            foreach(var request in candleServiceRequests)
            {
                CreateSubscription(request, candleUpdateConsumer);
            }
            _initialDataConsumer = initialDataConsumer;
            _candleRequests.AddRange(candleServiceRequests);
        }

        private void CreateSubscription(CandleServiceRequest serviceRequest, Action<IList<SingleCandleResponse>> candleUpdateConsumer)
        {
            Action<Candle> callback = (candle) => ProcessCandle(serviceRequest.RequestId, candle, candleUpdateConsumer);

            _candleProvider.Subscribe(serviceRequest.AssetPair, serviceRequest.CandleInterval, callback);
        }

        private void ProcessCandle(string requestId, Candle candle, Action<IList<SingleCandleResponse>> candleUpdateConsumer)
        {
            lock (_queueSync)
            {
                var response = new SingleCandleResponse
                {
                    RequestId = requestId,
                    Candle = candle
                };

                if (!_isHistoryDone)
                {
                    _responseQueue.Enqueue(response);
                    return;
                }

                candleUpdateConsumer(new List<SingleCandleResponse> { response });
            }
        }

        private void FillFromHistory(List<MultipleCandlesResponse> candles, List<CandleServiceRequest> requests)
        {
            lock (_queueSync)
            {
                for (var i = 0; i < candles.Count; i++)
                {
                    var response = candles[i];
                    var lastCandleInHistory = response.Candles[response.Candles.Count - 1];
                    var candlesForResponse = _responseQueue.Where(r => r.RequestId == response.RequestId && r.Candle.DateTime > lastCandleInHistory.DateTime).ToList();
                    var candleInterval = requests[i].CandleInterval;

                    DateTime targetDate;

                    if (candlesForResponse.Count > 0)
                        targetDate = candlesForResponse[0].Candle.DateTime;
                    else
                        targetDate = candleInterval.DecrementTimestamp(DateTime.UtcNow);

                    var nextDate = candleInterval.IncrementTimestamp(lastCandleInHistory.DateTime);

                    while (nextDate < targetDate)
                    {
                        var candle = new Candle();

                        candle.Open = candle.Close = candle.Low = candle.High = candle.LastTradePrice = lastCandleInHistory.Close;
                        candle.DateTime = nextDate;

                        nextDate = candleInterval.IncrementTimestamp(candle.DateTime);
                        response.Candles.Add(candle);
                    }

                    foreach (var candle in candlesForResponse)
                    {
                        response.Candles.Add(candle.Candle);
                    }

                    if (candlesForResponse.Count == 0)
                    {
                        _candleProvider.SetPrevCandleFromHistory(requests[i].AssetPair, candleInterval, response.Candles[response.Candles.Count - 1]);
                    }
                }

                _isHistoryDone = true;
            }
        }
    }
}
