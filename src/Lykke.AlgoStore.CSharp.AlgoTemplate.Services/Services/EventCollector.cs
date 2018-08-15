using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.AlgoStore.Service.InstanceEventHandler.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public sealed class EventCollector : IEventCollector
    {
        private readonly IAlgoSettingsService _settingsService;
        private readonly IInstanceEventHandlerClient _eventHandlerClient;

        private readonly BatchSubmitter<CandleChartingUpdate> _candleSubmitter;
        private readonly BatchSubmitter<FunctionChartingUpdate> _functionSubmitter;
        private readonly BatchSubmitter<TradeChartingUpdate> _tradeSubmitter;

        private readonly TimeSpan _maxBatchLifetime;
        private readonly int _batchSizeThreshold;

        private readonly bool _isBacktest;

        private bool _isDisposed;

        public EventCollector(
            IAlgoSettingsService settingsService,
            IInstanceEventHandlerClient eventHandlerClient,
            TimeSpan maxBatchLifetime,
            int batchSizeThreshold)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _eventHandlerClient = eventHandlerClient 
                ?? throw new ArgumentNullException(nameof(eventHandlerClient));

            _maxBatchLifetime = maxBatchLifetime;
            _batchSizeThreshold = batchSizeThreshold;

            _isBacktest = _settingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test;

            if(_isBacktest)
            {
                _candleSubmitter = MakeSubmitter<CandleChartingUpdate>(_eventHandlerClient.HandleCandlesAsync);
                _functionSubmitter = 
                    MakeSubmitter<FunctionChartingUpdate>(_eventHandlerClient.HandleFunctionsAsync);
                _tradeSubmitter = MakeSubmitter<TradeChartingUpdate>(_eventHandlerClient.HandleTradesAsync);
            }
        }

        public async Task SubmitCandleEvent(CandleChartingUpdate candle)
        {
            await PostEvent(_candleSubmitter, _eventHandlerClient.HandleCandlesAsync, candle);
        }

        public async Task SubmitFunctionEvent(FunctionChartingUpdate function)
        {
            await PostEvent(_functionSubmitter, _eventHandlerClient.HandleFunctionsAsync, function);
        }

        public async Task SubmitTradeEvent(TradeChartingUpdate trade)
        {
            await PostEvent(_tradeSubmitter, _eventHandlerClient.HandleTradesAsync, trade);
        }

        public async Task SubmitCandleEvents(IEnumerable<CandleChartingUpdate> candles)
        {
            await PostEvents(_candleSubmitter, _eventHandlerClient.HandleCandlesAsync, candles);
        }

        public async Task SubmitFunctionEvents(IEnumerable<FunctionChartingUpdate> functions)
        {
            await PostEvents(_functionSubmitter, _eventHandlerClient.HandleFunctionsAsync, functions);
        }

        public async Task SubmitTradeEvents(IEnumerable<TradeChartingUpdate> trades)
        {
            await PostEvents(_tradeSubmitter, _eventHandlerClient.HandleTradesAsync, trades);
        }

        public async Task SubmitQuoteEvent(QuoteChartingUpdate quote)
        {
            await PostEvents(_eventHandlerClient.HandleQuotesAsync, quote);
        }

        public async Task SubmitQuoteEvents(IEnumerable<QuoteChartingUpdate> quotes)
        {
            await PostEvents(_eventHandlerClient.HandleQuotesAsync, quotes);
        }

        private async Task PostEvents<T>(
            Func<List<T>, Task> eventHandlerMethod,
            T eventUpdate)
        {
            CheckDisposed();

            if (eventUpdate == null)
                throw new ArgumentNullException(nameof(eventUpdate));

            await eventHandlerMethod(new List<T> { eventUpdate });
        }

        private async Task PostEvents<T>(
            Func<List<T>, Task> eventHandlerMethod,
            IEnumerable<T> eventUpdates)
        {
            CheckDisposed();

            if (eventUpdates == null)
                throw new ArgumentNullException(nameof(eventUpdates));
            
            await eventHandlerMethod(eventUpdates.ToList());
        }


        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            _candleSubmitter?.Dispose();
            _functionSubmitter?.Dispose();
            _tradeSubmitter?.Dispose();
        }

        private async Task PostEvent<T>(
            BatchSubmitter<T> submitter,
            Func<List<T>, Task> eventHandlerMethod,
            T eventUpdate)
        {
            CheckDisposed();

            if (_isBacktest)
                submitter.Enqueue(eventUpdate);
            else
                await eventHandlerMethod(new List<T> { eventUpdate });
        }

        private async Task PostEvents<T>(
            BatchSubmitter<T> submitter,
            Func<List<T>, Task> eventHandlerMethod,
            IEnumerable<T> eventUpdates)
        {
            CheckDisposed();

            if (eventUpdates == null)
                throw new ArgumentNullException(nameof(eventUpdates));

            if (_isBacktest)
                submitter.Enqueue(eventUpdates);
            else
                await eventHandlerMethod(eventUpdates.ToList());
        }

        private void CheckDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(EventCollector));
        }

        private BatchSubmitter<T> MakeSubmitter<T>(Func<List<T>, Task> eventHandlerMethod)
        {
            return new BatchSubmitter<T>(
                _maxBatchLifetime, _batchSizeThreshold, MakeBatchHandler(eventHandlerMethod));
        }

        private Func<T[], Task> MakeBatchHandler<T>(Func<List<T>, Task> targetMethod)
        {
            return async (data) => await targetMethod(new List<T>(data));
        }
    }
}
