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
        private AlgoInstanceType _instanceType;

        public StatisticsService(IStatisticsRepository statisticsRepository, IAlgoSettingsService algoSettings)
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
            var data = new Statistics
            {
                InstanceId = _instanceId,
                Amount = volume,
                IsBuy = isBuy,
                Price = price,
                InstanceType = _instanceType
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public void OnAlgoStarted()
        {
            _instanceId = _algoSettings.GetInstanceId();
            _instanceType = _algoSettings.GetInstanceType();

            var data = new Statistics
            {
                InstanceId = _instanceId,
                InstanceType = _instanceType,
                IsStarted = true
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public StatisticsSummary GetSummary()
        {
            return _statisticsRepository.GetSummary(_instanceId, _instanceType).Result;
        }
    }
}
