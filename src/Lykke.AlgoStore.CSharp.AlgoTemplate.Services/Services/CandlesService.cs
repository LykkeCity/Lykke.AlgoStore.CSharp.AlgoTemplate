﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class CandlesService : ICandlesService
    {
        public class SubscriptionData
        {
            public Queue<SingleCandleResponse> ResponseQueue { get; } = new Queue<SingleCandleResponse>();
            public bool IsHistoryDone { get; set; }
            public object Sync { get; } = new object();
        }

        private readonly ICandleProviderService _candleProvider;
        private readonly IHistoryDataService _historyService;
        private readonly IAlgoSettingsService _settingsService;

        private bool _isProducing;
        private readonly List<CandleServiceRequest> _candleRequests = new List<CandleServiceRequest>();
        private Action<IList<MultipleCandlesResponse>> _initialDataConsumer;

        private readonly Dictionary<string, SubscriptionData> _subscriptions = new Dictionary<string, SubscriptionData>();

        public CandlesService(
            ICandleProviderService candleProvider,
            IHistoryDataService historyService, 
            IAlgoSettingsService settingsService)
        {
            _candleProvider = candleProvider;
            _historyService = historyService;
            _settingsService = settingsService;
        }

        public async Task StartProducing(CancellationToken cancellationToken)
        {
            if (_initialDataConsumer == null)
                throw new NotSupportedException("Cannot start producing with no subscriptions");

            if (_isProducing)
                throw new InvalidOperationException("The candle service has already started producing");

            await _candleProvider.Initialize();
            var candleProviderTask = _candleProvider.Start(cancellationToken);
            _isProducing = true;

            var isBacktest = _settingsService.GetSetting("InstanceType") == AlgoInstanceType.Test.ToString();

            var resultList = new List<MultipleCandlesResponse>();

            foreach(var request in _candleRequests)
            {
                if(isBacktest || request.IgnoreHistory)
                {
                    _subscriptions[request.RequestId].IsHistoryDone = true;
                    resultList.Add(new MultipleCandlesResponse
                    {
                        RequestId = request.RequestId,
                        Candles = Enumerable.Empty<Candle>()
                    });

                    continue;
                }

                var historyRequest = new CandlesHistoryRequest
                {
                    AuthToken = request.AuthToken,
                    IndicatorName = request.RequestId,
                    From = request.StartFrom,
                    To = request.EndOn,
                    AssetPair = request.AssetPair,
                    Interval = request.CandleInterval
                };

                var result = _historyService.GetHistory(historyRequest);
                var subscriptionData = _subscriptions[request.RequestId];

                resultList.Add(new MultipleCandlesResponse
                {
                    RequestId = request.RequestId,
                    Candles = new HistoryResultWrapperEnumerable(result, _candleProvider, historyRequest, subscriptionData)
                });
            }

            await Task.Yield();

            _initialDataConsumer(resultList);

            await candleProviderTask;
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
            var subscriptionData = new SubscriptionData();
            Action<Candle> callback = (candle) => ProcessCandle(serviceRequest.RequestId, candle, subscriptionData, candleUpdateConsumer);

            _subscriptions.Add(serviceRequest.RequestId, subscriptionData);

            _candleProvider.Subscribe(serviceRequest, callback);
        }

        private void ProcessCandle(string requestId, Candle candle, SubscriptionData subscriptionData,
                                   Action<IList<SingleCandleResponse>> candleUpdateConsumer)
        {
            lock (subscriptionData.Sync)
            {
                var response = new SingleCandleResponse
                {
                    RequestId = requestId,
                    Candle = candle
                };

                if (!subscriptionData.IsHistoryDone)
                {
                    subscriptionData.ResponseQueue.Enqueue(response);
                    return;
                }

                candleUpdateConsumer(new List<SingleCandleResponse> { response });
            }
        }
    }
}
