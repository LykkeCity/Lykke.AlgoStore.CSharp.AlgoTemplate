using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Used when providing the initial data from <see cref="CandlesService"/>.
    /// After the end of the wrapped enumerator, provides queued incoming candles from a <see cref="ICandleProviderService"/>
    /// </summary>
    /// <remarks>
    /// This class handles the case when we receive new candles while we're processing historical data - incoming
    /// candles are queued and passed as part of the warmup to prevent missing candles and thus preventing the algorithm
    /// from entering an invalid state
    /// </remarks>
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

        /// <summary>
        /// Retrieves the next <see cref="Candle"/> from the incoming candles queue
        /// </summary>
        /// <returns>True if there are more <see cref="Candle"/>s in the queue, false otherwise</returns>
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

                    if(_queueEmpty && _currentCandle != null)
                        _candleProvider.SetPrevCandleFromHistory(_candlesHistoryRequest.AssetPair, _candlesHistoryRequest.Interval, _currentCandle);
                }
            }

            _currentCandle = null;
            return false;
        }
    }
}
