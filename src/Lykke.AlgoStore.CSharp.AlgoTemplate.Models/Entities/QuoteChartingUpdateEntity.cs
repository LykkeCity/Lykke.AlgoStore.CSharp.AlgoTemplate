using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class QuoteChartingUpdateEntity : TableEntity
    {
        public string InstanceId { get; set; }
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public double Price { get; set; }
        public DateTime QuoteTimestamp { get; set; }
        public DateTime DateReceived { get; set; }
        public bool IsOnline { get; set; }
    }
}
