using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoClientInstanceEntity : TableEntity
    {
        public string HftApiKey { get; set; }
        public string ClientId { get; set; }
        public string AlgoId { get; set; }
        public string AssetPair { get; set; }
        public string TradedAsset { get; set; }
        public double Volume { get; set; }
        public double Margin { get; set; }
        public string AlgoMetaDataInformation { get; set; }
        public string WalletId { get; set; }
    }
}
