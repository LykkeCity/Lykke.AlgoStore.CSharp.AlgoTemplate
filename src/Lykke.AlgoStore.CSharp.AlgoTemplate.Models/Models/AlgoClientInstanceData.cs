using System;
using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoClientInstanceData : BaseAlgoInstance
    {
        public string HftApiKey { get; set; }

        [Required]
        public string WalletId { get; set; }

        [Required]
        public string AssetPair { get; set; }

        [Required]
        public string TradedAsset { get; set; }

        [Range(Double.Epsilon, double.MaxValue)]
        public double Volume { get; set; }

        public double Margin { get; set; }

        public string InstanceName { get; set; }

        [Required]
        public AlgoMetaDataInformation AlgoMetaDataInformation { get; set; }
    }
}
