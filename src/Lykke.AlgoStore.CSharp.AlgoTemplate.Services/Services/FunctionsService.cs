using System;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class FunctionsService : IFunctionsService
    {
        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public List<CandlesHistoryRequest> GetRequest()
        {
            throw new NotImplementedException();
        }

        public void WarmUp(List<Candle> candles)
        {
            throw new NotImplementedException();
        }

        public void Calculate(IAlgoQuote quote)
        {
            throw new NotImplementedException();
        }

        public double GetValue(string functionName)
        {
            throw new NotImplementedException();
        }
    }
}
