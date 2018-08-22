using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class QuoteChartingUpdateData
    {
        public string InstanceId { get; set; }
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime DateReceived { get; set; }
        public bool IsOnline { get; set; }
    }
}
