using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AzureStorage.Tables;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoInstanceTradeEntity : TableEntity
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

        public string OrderTypeValue { get; set; }

        public OrderType OrderType
        {
            get
            {
                OrderType type = 0;
                Enum.TryParse(OrderTypeValue, out type);
                return type;
            }
            set => OrderTypeValue = value.ToString();
         }

        public string OrderStatusValue { get; set; }

        public OrderStatus OrderStatus
        {
            get
            {
                OrderStatus status = 0;
                Enum.TryParse(OrderStatusValue, out status);
                return status;
            }
            set => OrderStatusValue = value.ToString();
        }
    }
}
