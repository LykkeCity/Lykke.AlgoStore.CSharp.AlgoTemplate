using System;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IStatisticsService"/> implementation
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoSettingsService _algoSettings;
        private readonly IEventCollector _eventCollector;
        private string _instanceId;
        private string _algoAssetPair;

        public StatisticsService(
            IStatisticsRepository statisticsRepository,
            IAlgoSettingsService algoSettings,
            IEventCollector eventCollector)
        {
            _statisticsRepository = statisticsRepository;
            _algoSettings = algoSettings;
            _algoAssetPair = _algoSettings.GetAlgoInstanceAssetPairId();
            _eventCollector = eventCollector;
        }

        public IAlgoQuote GetQuote()
        {
            throw new NotImplementedException();
        }

        public void OnQuote(IAlgoQuote quote)
        {
            var candleChartingUpdate = AutoMapper.Mapper.Map<QuoteChartingUpdate>(quote);

            candleChartingUpdate.InstanceId = _algoSettings.GetInstanceId();
            candleChartingUpdate.AssetPair = _algoAssetPair;

            _eventCollector.SubmitQuoteEvent(candleChartingUpdate).GetAwaiter().GetResult();
        }

        public void OnAction(bool isBuy, double volume, double price)
        {
            // Live instances trade count gets adjusted by the algo trades service
            if (_algoSettings.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Live) return;

            var summary = GetSummary();

            summary.TotalNumberOfTrades++;

            _statisticsRepository.CreateAsync(summary).Wait();
        }

        public void OnAlgoStarted()
        {
            _instanceId = _algoSettings.GetInstanceId();

            var summary = GetSummary();

            //REMARK: This should never happen when we implement summary math (before algo instance is created)
            if (summary == null)
            {
                CreateDefaultSummary();
                summary = GetSummary();
            }

            summary.TotalNumberOfStarts++;

            _statisticsRepository.CreateAsync(summary).Wait();
        }

        public void OnAlgoStopped()
        {

        }

        public StatisticsSummary GetSummary()
        {
            return _statisticsRepository.GetSummaryAsync(_instanceId).Result;
        }

        private void CreateDefaultSummary()
        {
            var summary = new StatisticsSummary
            {
                InstanceId = _instanceId,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0
            };

            _statisticsRepository.CreateOrUpdateSummaryAsync(summary).Wait();
        }
    }
}
