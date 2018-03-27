using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.Service.CandlesHistory.Client;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
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

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _buffer = null;
                _isDisposed = true;
            }
        }

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

                if (_nextTimestamp > DateTime.UtcNow)
                {
                    _nextTimestamp = DateTime.UtcNow;
                    _isLastBuffer = true;
                }

                FillBuffer();
            }
            while ((_buffer == null || _buffer.Count == 0) && !_isLastBuffer);
        }

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
