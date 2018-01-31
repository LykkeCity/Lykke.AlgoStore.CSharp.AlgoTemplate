using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class CandlesHistoryRequest
    {
        public string AssetPair { get; set; }
        public CandlePriceType PriceType { get; set; }
        public CandleTimeInterval Interval { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
