using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IHistoryDataService
    {
        List<Candle> GetHistoryCandles(CandlesHistoryRequest request);
    }
}
