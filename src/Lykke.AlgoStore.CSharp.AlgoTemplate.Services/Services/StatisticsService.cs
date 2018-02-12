using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
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

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
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
                //InstanceId = 
                Amount = volume,
                //Id = 
                IsBought = isBuy,
                Price = price
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public double GetBoughtAmount()
        {
            throw new NotImplementedException();
        }

        public double GetSoldAmount()
        {
            throw new NotImplementedException();
        }

        public double GetBoughtQuantity()
        {
            throw new NotImplementedException();
        }

        public double GetSoldQuantity()
        {
            throw new NotImplementedException();
        }
    }
}
