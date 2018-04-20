namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoInstanceTrade
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
