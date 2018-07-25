using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using System;
using System.Collections.Generic;
using Lykke.AlgoStore.Service.History.Client;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Rest;
using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Used to retrieve candles from the history service in batches, thereby preventing
    /// loading too many candles into memory at one time
    /// </summary>
    public class CandleHistoryBatchEnumerator : CandleGapFillEnumeratorBase
    {
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly IHistoryClient _historyClient;

        private IList<Service.History.Client.Models.Candle> _buffer = new List<Service.History.Client.Models.Candle>();
        private DateTime _currentTimestamp;
        private DateTime _nextTimestamp;

        private int _currentIndex = 0;

        private bool _isLastBuffer;
        private bool _isDisposed;

        public CandleHistoryBatchEnumerator(CandlesHistoryRequest historyRequest, IHistoryClient historyClient) : base(historyRequest.Interval)
        {
            _candlesHistoryRequest = historyRequest;
            _historyClient = historyClient;

            // If the from date is after the current moment, immediately set the last buffer flag
            // so that the enumerator becomes empty
            if (_candlesHistoryRequest.From > DateTime.UtcNow)
                _isLastBuffer = true;
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
                DateTime = DateTime.SpecifyKind(candle.DateTime, DateTimeKind.Utc),
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

            IncrementBuffer().ConfigureAwait(false).GetAwaiter().GetResult();

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
        private async Task IncrementBuffer()
        {
            if (_isLastBuffer) return;

            do
            {
                if (_currentTimestamp == default(DateTime))
                    _currentTimestamp = _candlesHistoryRequest.From;
                else
                    _currentTimestamp = _nextTimestamp;

                _nextTimestamp = _candlesHistoryRequest.Interval.IncrementTimestamp(_currentTimestamp, 5000);

                var timeLimit = DateTime.UtcNow > _candlesHistoryRequest.To ? _candlesHistoryRequest.To : DateTime.UtcNow;

                if (_nextTimestamp > timeLimit)
                {
                    _nextTimestamp = timeLimit;
                    _isLastBuffer = true;
                }

                await FillBuffer();
            }
            while ((_buffer == null || _buffer.Count == 0) && !_isLastBuffer);
        }

        /// <summary>
        /// Fills the buffer from the history service
        /// </summary>
        private async Task FillBuffer()
        {
            IEnumerable<Service.History.Client.Models.Candle> history = null;

            while (history == null)
            {
                try
                {
                    history = await _historyClient.GetCandles(
                        _currentTimestamp, _nextTimestamp,
                        _candlesHistoryRequest.AssetPair,
                        (Service.History.Client.Models.CandleTimeInterval)_candlesHistoryRequest.Interval,
                        _candlesHistoryRequest.IndicatorName,
                        _candlesHistoryRequest.AuthToken);

                    _buffer = history.ToList();
                    _currentIndex = 0;
                }
                catch(TaskCanceledException)
                {
                    // Seems to be caused by a timeout, wait for a while and retry
                    await Task.Delay(TimeSpan.FromSeconds(60));
                }
                catch (HttpOperationException e)
                {
                    if (e.Response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                        e.Response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        var retryAfter = int.Parse(e.Response.Headers["Retry-After"].First());

                        await Task.Delay(TimeSpan.FromSeconds(retryAfter));
                    }
                    else throw;
                }
            }
        }
    }
}
