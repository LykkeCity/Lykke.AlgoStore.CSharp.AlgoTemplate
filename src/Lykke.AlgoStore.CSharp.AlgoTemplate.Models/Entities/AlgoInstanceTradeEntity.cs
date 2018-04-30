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

        public string WalletId { get; set; }
    }
}
