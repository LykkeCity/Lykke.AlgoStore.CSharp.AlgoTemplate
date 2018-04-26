using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.Service.CandlesHistory.Client;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Used to retrieve candles from the history service in batches, thereby preventing
    /// loading too many candles into memory at one time
    /// </summary>
    public class CandleHistoryBatchEnumerator : CandleGapFillEnumeratorBase
    {
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly ICandleshistoryservice _candlesHistoryService;

        private IList<Service.CandlesHistory.Client.Models.Candle> _buffer = new List<Service.CandlesHistory.Client.Models.Candle>();
        private DateTime _currentTimestamp;
        private DateTime _nextTimestamp;

        private int _currentIndex = 0;
        
        private bool _isLastBuffer;
        private bool _isDisposed;

        public CandleHistoryBatchEnumerator(CandlesHistoryRequest historyRequest, ICandleshistoryservice candlesHistoryService) : base(historyRequest.Interval)
        {
            _candlesHistoryRequest = historyRequest;
            _candlesHistoryService = candlesHistoryService;
        }

        /// <summary>
        /// Returns the current <see cref="Candle"/>
        /// </summary>
        /// <returns>The current <see cref="Candle"/></returns>
        protected override Candle GetCurrent()
        {
            var candle = _buffer[_currentIndex];

            return new Candle
            {
                Open = candle.Open,
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low,
                DateTime = candle.DateTime,
                LastTradePrice = candle.LastTradePrice,
                TradingOppositeVolume = candle.TradingOppositeVolume,
                TradingVolume = candle.TradingVolume
            };
        }

        /// <summary>
        /// Moves the current <see cref="Candle"/> one index forward
        /// </summary>
        /// <returns>True if the next <see cref="Candle"/> has been loaded, false if there are no more candles to load</returns>
        protected override bool MoveNextIndex()
        {
            if (_currentIndex < _buffer.Count - 1)
            {
                _currentIndex++;
                return true;
            }

            if (_isLastBuffer)
                return ++_currentIndex < _buffer.Count;

            IncrementBuffer();

            return !_isLastBuffer || _buffer.Count > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _buffer = null;
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Sets the range and fetches the next buffer from the history service
        /// </summary>
        private void IncrementBuffer()
        {
            if (_isLastBuffer) return;

            do
            {
                if (_currentTimestamp == default(DateTime))
                    _currentTimestamp = _candlesHistoryRequest.From;
                else
                    _currentTimestamp = _nextTimestamp;

                _nextTimestamp = _candlesHistoryRequest.Interval.IncrementTimestamp(_currentTimestamp, 9999);

                var timeLimit = DateTime.UtcNow > _candlesHistoryRequest.To ? _candlesHistoryRequest.To : DateTime.UtcNow;

                if (_nextTimestamp > timeLimit)
                {
                    _nextTimestamp = timeLimit;
                    _isLastBuffer = true;
                }

                FillBuffer();
            }
            while ((_buffer == null || _buffer.Count == 0) && !_isLastBuffer);
        }

        /// <summary>
        /// Fills the buffer from the history service
        /// </summary>
        private void FillBuffer()
        {
            var task = _candlesHistoryService.GetCandlesHistoryAsync(_candlesHistoryRequest.AssetPair, Service.CandlesHistory.Client.Models.CandlePriceType.Mid,
                                                                    (Service.CandlesHistory.Client.Models.CandleTimeInterval)_candlesHistoryRequest.Interval,
                                                                    _currentTimestamp, _nextTimestamp);

            task.Wait();

            _buffer = task.Result.History;
            _currentIndex = 0;
        }
    }
}
