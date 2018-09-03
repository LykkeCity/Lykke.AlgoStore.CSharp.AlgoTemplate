using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AzureStorage.Tables;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoInstanceTradeEntity : AzureTableEntity
    {
        public string InstanceId { get; set; }

        public bool? IsBuy { get; set; }

        public double? Price { get; set; }

        public double? Amount { get; set; }

        public double? Fee { get; set; }

        public string OrderId { get; set; }

        public string AssetPairId { get; set; }

        public string AssetId { get; set; }
        
        public string WalletId { get; set; }

        public DateTime? DateOfTrade { get; set; }

        public OrderType OrderType { get; set; }
    }
}
