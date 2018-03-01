using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

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
                Price = price
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public void OnAlgoStarted()
        {
            //REMARK: No need to save any statistics here (for now)

            _instanceId = _algoSettings.GetSetting("InstanceId");
        }

        public double GetBoughtAmount()
        {
            return _statisticsRepository.GetBoughtAmountAsync(_instanceId).Result;
        }

        public double GetSoldAmount()
        {
            return _statisticsRepository.GetSoldAmountAsync(_instanceId).Result;
        }

        public double GetBoughtQuantity()
        {
            return _statisticsRepository.GetBoughtQuantityAsync(_instanceId).Result;
        }

        public double GetSoldQuantity()
        {
            return _statisticsRepository.GetSoldQuantityAsync(_instanceId).Result;
        }

        public int GetNumberOfRunningsForAnAlgo()
        {
            return _statisticsRepository.GetNumberOfRunnings(_instanceId).Result;
        }
    }
}
