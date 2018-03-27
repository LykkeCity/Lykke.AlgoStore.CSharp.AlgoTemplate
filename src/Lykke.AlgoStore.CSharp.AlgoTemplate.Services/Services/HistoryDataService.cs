using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.Service.CandlesHistory.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
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
