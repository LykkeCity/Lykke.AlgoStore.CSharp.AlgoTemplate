using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    public class MultipleCandlesResponse
    {
        public string RequestId { get; set; }

        public IEnumerable<Candle> Candles { get; set; }
    }
}
