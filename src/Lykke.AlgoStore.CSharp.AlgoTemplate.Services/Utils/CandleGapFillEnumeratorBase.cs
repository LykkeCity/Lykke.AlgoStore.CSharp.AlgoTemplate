using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Base class containing logic to generate missing candles from a provider
    /// </summary>
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

        /// <summary>
        /// Contains the main logic to fill the empty gaps between returned candles.
        /// </summary>
        /// <remarks>
        /// For example, if we have two candles for an interval of one day, one on 2018-01-01 and the other on 2018-01-03,
        /// this method will fill the gap between the two and generate an empty candle for 2018-01-02
        /// </remarks>
        /// <returns>True if the next <see cref="Candle"/> has been loaded, false if there aren't any more to load</returns>
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

        /// <summary>
        /// Support for this method isn't required. See https://msdn.microsoft.com/en-us/library/78dfe2yb.aspx#Remarks
        /// for more information
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        /// <summary>
        /// Verifies the state of the enumerator and returns the current <see cref="Candle"/>
        /// </summary>
        /// <returns>The current <see cref="Candle"/></returns>
        private Candle CheckStateAndGetCurrent()
        {
            CheckDisposed();

            if (!_isStarted || _isEnumeratorDone)
                return null;

            if (_intermediateCandle != null)
                return _intermediateCandle;

            return GetCurrent();
        }

        /// <summary>
        /// Updates the current and previous <see cref="Candle"/> and moves to next element
        /// </summary>
        /// <returns>True if the next <see cref="Candle"/> has been loaded, false if there aren't any more to load</returns>
        private bool SetCandlesAndMoveIndex()
        {
            _prevCandle = _currentCandle;
            var result = MoveNextIndex();

            if (result)
                _currentCandle = Current;

            _isEnumeratorDone = !result;

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
