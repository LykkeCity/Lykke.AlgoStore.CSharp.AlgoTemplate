using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFunctionsService : IFunctions
    {
        void Initialise();
        List<CandlesHistoryRequest> GetRequest();
        void WarmUp(List<Candle> candles);
        void Calculate(IAlgoQuote quote);
    }
}
