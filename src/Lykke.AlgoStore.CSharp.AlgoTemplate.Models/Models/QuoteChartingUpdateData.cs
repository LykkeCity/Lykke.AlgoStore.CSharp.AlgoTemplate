using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class QuoteChartingUpdateData
    {
        public string InstanceId { get; set; }
        public string AssetPair { get; set; }
        public bool IsBuy { get; }
        public double Price { get; }
        public DateTime Timestamp { get; }
        public DateTime DateReceived { get; }
        public bool IsOnline { get; }
    }
}
