using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using System.Collections;
using System.Collections.Generic;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Dummy class, needed to inherit from the <see cref="IEnumerable{T}"/> interface and 
    /// provide access to the <see cref="CandleHistoryBatchEnumerator"/>
    /// </summary>
    public class CandleHistoryBatchEnumerable : IEnumerable<Candle>
    {
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly IHistoryClient _historyClient;

        public CandleHistoryBatchEnumerable(CandlesHistoryRequest historyRequest, IHistoryClient historyClient)
        {
            _candlesHistoryRequest = historyRequest;
            _historyClient = historyClient;
        }

        public IEnumerator<Candle> GetEnumerator()
        {
            return new CandleHistoryBatchEnumerator(_candlesHistoryRequest, _historyClient);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CandleHistoryBatchEnumerator(_candlesHistoryRequest, _historyClient);
        }
    }
}
