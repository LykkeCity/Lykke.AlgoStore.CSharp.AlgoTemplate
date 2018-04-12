using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class StatisticsSummaryEntity : TableEntity
    {
        public string InstanceId { get; set; }

        public int TotalNumberOfTrades { get; set; }

        public int TotalNumberOfStarts { get; set; }

        public double InitialWalletBalance { get; set; }

        public double LastWalletBalance { get; set; }

        public double InitialTradedAssetBalance { get; set; }

        public double InitialAssetTwoBalance { get; set; }

        public double LastTradedAssetBalance { get; set; }

        public double LastAssetTwoBalance { get; set; }

        public string UserCurrencyBaseAssetId { get; set; }
    }
}
