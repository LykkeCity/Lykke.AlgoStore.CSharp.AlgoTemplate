using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    public class MultipleCandlesResponse
    {
        public string RequestId { get; set; }

        public IEnumerable<Candle> Candles { get; set; }
    }
}
