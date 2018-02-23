using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly string _instanceId;

        public StatisticsService(IStatisticsRepository statisticsRepository, string instanceId)
        {
            _statisticsRepository = statisticsRepository;
            _instanceId = instanceId;
        }

        public IAlgoQuote GetQuote()
        {
            throw new NotImplementedException();
        }

        public void OnQuote(IAlgoQuote quote)
        {
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
            var data = new Statistics { IsStarted = true };

            _statisticsRepository.CreateAsync(data).Wait();
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
