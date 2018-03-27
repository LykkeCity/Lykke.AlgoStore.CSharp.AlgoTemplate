using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.Service.CandlesHistory.Client;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public class CandleHistoryBatchEnumerable : IEnumerable<Candle>
    {
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly ICandleshistoryservice _candlesHistoryService;

        public CandleHistoryBatchEnumerable(CandlesHistoryRequest historyRequest, ICandleshistoryservice candlesHistoryService)
        {
            _candlesHistoryRequest = historyRequest;
            _candlesHistoryService = candlesHistoryService;
        }

        public IEnumerator<Candle> GetEnumerator()
        {
            return new CandleHistoryBatchEnumerator(_candlesHistoryRequest, _candlesHistoryService);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CandleHistoryBatchEnumerator(_candlesHistoryRequest, _candlesHistoryService);
        }
    }
}
