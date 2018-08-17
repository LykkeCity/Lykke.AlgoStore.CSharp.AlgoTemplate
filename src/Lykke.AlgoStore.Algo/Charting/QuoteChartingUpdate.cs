using System;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class QuoteChartingUpdate : IAlgoQuote, IChartingUpdate
    {
        public bool IsBuy { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime DateReceived { get; set; }
        public bool IsOnline { get; set; }
        public string InstanceId { get; set; }
        public string AssetPair { get; set; }
    }
}
