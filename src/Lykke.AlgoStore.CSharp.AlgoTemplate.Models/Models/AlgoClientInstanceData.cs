using System;
using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoClientInstanceData : BaseAlgoInstance
    {
        public string HftApiKey { get; set; }

        public string WalletId { get; set; }

        [Required]
        public string AssetPair { get; set; }       

        [Required]
        public string TradedAsset { get; set; }

        public string OppositeAssetId { get; set; }

        [Range(Double.Epsilon, double.MaxValue)]
        public double Volume { get; set; }

        public double Margin { get; set; }

        public string InstanceName { get; set; }
        
        public DateTime? AlgoInstanceRunDate { get; set; }

        public bool IsStraight { get; set; }

        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }

        public AlgoInstanceType AlgoInstanceType { get; set; }

        [Required]
        public AlgoMetaDataInformation AlgoMetaDataInformation { get; set; }

        public string AlgoClientId { get; set; }

        public double BackTestTradingAssetBalance { get; set; }

        public double BackTestAssetTwoBalance { get; set; }

        public string AuthToken { get; set; }
    }
}
