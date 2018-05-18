using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService
{
    public class SingleCandleResponse
    {
        public string RequestId { get; set; }

        public Candle Candle { get; set; }
    }
}
