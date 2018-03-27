using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public abstract class CandleGapFillEnumeratorBase : IEnumerator<Candle>
    {
        private bool _isDisposed;
        private bool _isEnumeratorDone;
        private bool _isStarted;

        private Candle _currentCandle;
        private Candle _intermediateCandle;
        private Candle _prevCandle;

        private CandleTimeInterval _candleTimeInterval;

        public Candle Current => CheckStateAndGetCurrent();
        object IEnumerator.Current => CheckStateAndGetCurrent();

        public CandleGapFillEnumeratorBase(CandleTimeInterval candleTimeInterval)
        {
            _candleTimeInterval = candleTimeInterval;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool MoveNext()
        {
            CheckDisposed();

            if (!_isStarted)
            {
                _isStarted = true;
                return SetCandlesAndMoveIndex();
            }

            // If this is the first candle, or there is no gap between the current and the last, continue
            if (_prevCandle == null || HasNoGapBetweenCandles())
            {
                var result = SetCandlesAndMoveIndex();

                if (!result || HasNoGapBetweenCandles())
                    return result;
            }

            // If there's no intermediate candle, create it and return
            if (_intermediateCandle == null)
            {
                _intermediateCandle = new Candle();

                _intermediateCandle.Open = _intermediateCandle.Close = _intermediateCandle.Low = _intermediateCandle.High = _prevCandle.Close;
                _intermediateCandle.DateTime = _candleTimeInterval.IncrementTimestamp(_prevCandle.DateTime);

                return true;
            }

            if (_intermediateCandle.DateTime < _currentCandle.DateTime)
            {
                var nextDate = _candleTimeInterval.IncrementTimestamp(_intermediateCandle.DateTime);

                // If the next intermediate date matches the current candle, clear intermediate and return
                if (nextDate == _currentCandle.DateTime)
                {
                    _prevCandle = _intermediateCandle;
                    _intermediateCandle = null;

                    return true;
                }

                // Otherwise, increment intermediate
                var newIntermediate = new Candle();
                newIntermediate.Open = newIntermediate.Close = newIntermediate.Low = newIntermediate.High = _intermediateCandle.Close;
                newIntermediate.DateTime = nextDate;

                _intermediateCandle = newIntermediate;

                return true;
            }

            return SetCandlesAndMoveIndex();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        private Candle CheckStateAndGetCurrent()
        {
            CheckDisposed();

            if (!_isStarted)
                return null;

            if (_isEnumeratorDone)
                throw new InvalidOperationException("The enumerator is past the final element");

            if (_intermediateCandle != null)
                return _intermediateCandle;

            return GetCurrent();
        }

        private bool SetCandlesAndMoveIndex()
        {
            _prevCandle = _currentCandle;
            var result = MoveNextIndex();

            if (result)
                _currentCandle = Current;

            return result;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new InvalidOperationException("The enumerator has been disposed.");
        }

        private bool HasNoGapBetweenCandles()
        {
            return _candleTimeInterval.IncrementTimestamp(_prevCandle.DateTime) == _currentCandle.DateTime;
        }

        protected abstract bool MoveNextIndex();
        protected abstract Candle GetCurrent();
    }
}
