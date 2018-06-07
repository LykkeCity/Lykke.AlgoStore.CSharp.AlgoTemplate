using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.Service.CandlesHistory.Client;
using System.Collections;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Dummy class, needed to inherit from the <see cref="IEnumerable{T}"/> interface and 
    /// provide access to the <see cref="CandleHistoryBatchEnumerator"/>
    /// </summary>
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
