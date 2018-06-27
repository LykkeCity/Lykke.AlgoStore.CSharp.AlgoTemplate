﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoClientInstanceEntity : TableEntity
    {
        public string HftApiKey { get; set; }
        public string ClientId { get; set; }
        public string AlgoId { get; set; }
        public string AssetPairId { get; set; }
        public string TradedAssetId { get; set; }
        public string OppositeAssetId { get; set; }
        public double Volume { get; set; }
        public double Margin { get; set; }
        public string AlgoMetaDataInformation { get; set; }
        public string WalletId { get; set; }
        public string InstanceName { get; set; }
        public bool IsStraight { get; set; }

        public DateTime AlgoInstanceCreateDate { get; set; }
        public DateTime? AlgoInstanceRunDate { get; set; }
        public DateTime? AlgoInstanceStopDate { get; set; }

        public string AlgoInstanceStatusValue { get; set; }
        public string AlgoInstanceTypeValue { get; set; }

        public AlgoInstanceStatus AlgoInstanceStatus
        {
            get
            {
                AlgoInstanceStatus type = 0;
                Enum.TryParse(AlgoInstanceStatusValue, out type);
                return type;
            }
            set => AlgoInstanceStatusValue = value.ToString();
        }

        public AlgoInstanceType AlgoInstanceType
        {
            get
            {
                AlgoInstanceType type = 0;
                Enum.TryParse(AlgoInstanceTypeValue, out type);
                return type;
            }
            set => AlgoInstanceTypeValue = value.ToString();
        }

        public string AlgoClientId { get; set; }
        public string AuthToken { get; set; }
    }
}
