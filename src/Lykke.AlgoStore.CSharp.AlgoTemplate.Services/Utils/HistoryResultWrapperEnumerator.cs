using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public class HistoryResultWrapperEnumerator : CandleGapFillEnumeratorBase
    {
        private readonly IEnumerator<Candle> _originalEnumerator;
        private readonly ICandleProviderService _candleProvider;
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly CandlesService.SubscriptionData _subscriptionData;

        private Candle _currentCandle;

        private bool _queueEmpty = true;

        public HistoryResultWrapperEnumerator(IEnumerator<Candle> originalEnumerator,
                                              ICandleProviderService candleProvider,
                                              CandlesHistoryRequest candlesHistoryRequest,
                                              CandlesService.SubscriptionData subscriptionData)
            : base(candlesHistoryRequest.Interval)
        {
            _originalEnumerator = originalEnumerator;
            _candleProvider = candleProvider;
            _candlesHistoryRequest = candlesHistoryRequest;
            _subscriptionData = subscriptionData;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _originalEnumerator.Dispose();
            }
        }

        protected override Candle GetCurrent()
        {
            return _currentCandle;
        }

        protected override bool MoveNextIndex()
        {
            if(_originalEnumerator.MoveNext())
            {
                _currentCandle = _originalEnumerator.Current;
                return true;
            }

            lock(_subscriptionData.Sync)
            {
                if(_subscriptionData.ResponseQueue.Count > 0)
                {
                    Candle candle = null;
                    _queueEmpty = false;

                    do
                    {
                        candle = _subscriptionData.ResponseQueue.Dequeue().Candle;
                    }
                    while (_subscriptionData.ResponseQueue.Count > 0 && candle.DateTime < _currentCandle.DateTime);
                    
                    _currentCandle = candle;
                    return true;
                }
                else
                {
                    _subscriptionData.IsHistoryDone = true;

                    if(_queueEmpty)
                        _candleProvider.SetPrevCandleFromHistory(_candlesHistoryRequest.AssetPair, _candlesHistoryRequest.Interval, _currentCandle);
                }
            }

            _currentCandle = null;
            return false;
        }
    }
}
