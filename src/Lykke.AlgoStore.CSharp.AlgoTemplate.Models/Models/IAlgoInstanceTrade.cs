using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public interface IAlgoInstanceTrade
    {
        string InstanceId { get; set; }
        string Id { get; set; }
        bool? IsBuy { get; set; }
        double? Price { get; set; }
        double? Amount { get; set; }
        double? Fee { get; set; }
        string OrderId { get; set; }
        string AssetPairId { get; set; }
        DateTime? DateOfTrade { get; set; }
        string AssetId { get; set; }
    }
}
