using Lykke.AlgoStore.Algo;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    public class MultipleCandlesResponse
    {
        public string RequestId { get; set; }

        public IEnumerable<Candle> Candles { get; set; }
    }
}
