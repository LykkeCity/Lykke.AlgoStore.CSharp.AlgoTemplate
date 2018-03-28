using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.Service.CandlesHistory.Client;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// Mostly dummy class, main logic of the history data service is 
    /// in the <see cref="CandleHistoryBatchEnumerator"/>
    /// </summary>
    public class HistoryDataService : IHistoryDataService
    {
        private readonly ICandleshistoryservice _candlesHistoryService;

        public HistoryDataService(ICandleshistoryservice candlesHistoryService)
        {
            _candlesHistoryService = candlesHistoryService;
        }

        public IEnumerable<Candle> GetHistory(CandlesHistoryRequest request)
        {
            return new CandleHistoryBatchEnumerable(request, _candlesHistoryService);
        }
    }
}
