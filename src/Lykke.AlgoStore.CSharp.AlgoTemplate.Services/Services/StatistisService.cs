using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class StatistisService : IStatisticsService
    {
        public IAlgoQuote GetQuote()
        {
            throw new NotImplementedException();
        }

        public void OnQuote(IAlgoQuote quote)
        {
            throw new NotImplementedException();
        }

        public void OnAction(bool isBuy, double volume)
        {
            throw new NotImplementedException();
        }
    }
}
