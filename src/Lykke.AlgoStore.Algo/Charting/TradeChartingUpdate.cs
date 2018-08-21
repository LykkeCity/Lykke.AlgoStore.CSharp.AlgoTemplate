using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class TradeChartingUpdate : IAlgoInstanceTrade, IChartingUpdate
    {
        public string InstanceId { get; set; }
        public string Id { get; set; }
        public bool? IsBuy { get; set; }
        public double? Price { get; set; }
        public double? Amount { get; set; }
        public double? Fee { get; set; }
        public string OrderId { get; set; }
        public string AssetPairId { get; set; }
        public DateTime? DateOfTrade { get; set; }
        public string AssetId { get; set; }
    }
}
