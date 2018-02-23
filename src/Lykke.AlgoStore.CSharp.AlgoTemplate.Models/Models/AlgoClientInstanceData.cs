﻿using System;
using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoClientInstanceData : BaseAlgoInstance
    {
        [Required]
        public string HftApiKey { get; set; }
        [Required]
        public string AssetPair { get; set; }
        [Required]
        public string TradedAsset { get; set; }
        [Range(Double.Epsilon, double.MaxValue)]
        public double Volume { get; set; }
        [Required]
        public double Margin { get; set; }
        [Required]
        public AlgoMetaDataInformation AlgoMetaDataInformation { get; set; }
    }
}
