using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    public class SingleCandleResponse
    {
        public string RequestId { get; set; }

        public Candle Candle { get; set; }
    }
}
