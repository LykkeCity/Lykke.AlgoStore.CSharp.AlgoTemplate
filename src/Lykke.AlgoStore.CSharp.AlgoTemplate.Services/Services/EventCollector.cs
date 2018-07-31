using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.AlgoStore.Service.InstanceEventHandler.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class EventCollector : IEventCollector
    {
        private readonly IAlgoSettingsService _settingsService;
        private readonly IInstanceEventHandlerClient _eventHandlerClient;

        private readonly BatchSubmitter<CandleChartingUpdate> _candleSubmitter;
        private readonly BatchSubmitter<FunctionChartingUpdate> _functionSubmitter;
        private readonly BatchSubmitter<TradeChartingUpdate> _tradeSubmitter;

        private readonly TimeSpan _maxBatchLifetime;
        private readonly int _batchSizeThreshold;

        private readonly bool _isBacktest;

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

        private async Task PostEvent<T>(
            BatchSubmitter<T> submitter,
            Func<List<T>, Task> eventHandlerMethod,
            T eventUpdate)
        {
            if (_isBacktest)
                submitter.Enqueue(eventUpdate);
            else
                await eventHandlerMethod(new List<T> { eventUpdate });
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
