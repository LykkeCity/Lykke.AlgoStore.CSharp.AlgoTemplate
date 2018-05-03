using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
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
        private string _instanceId;

        public StatisticsService(
            IStatisticsRepository statisticsRepository,
            IAlgoSettingsService algoSettings)
        {
            _statisticsRepository = statisticsRepository;
            _algoSettings = algoSettings;
        }

        public IAlgoQuote GetQuote()
        {
            throw new NotImplementedException();
        }

        public void OnQuote(IAlgoQuote quote)
        {
            //REMARK: No need to save any statistics here (for now)
        }

        public void OnAction(bool isBuy, double volume, double price)
        {
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
