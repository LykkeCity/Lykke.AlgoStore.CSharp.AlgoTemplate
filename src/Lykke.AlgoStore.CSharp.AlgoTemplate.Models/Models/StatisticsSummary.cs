﻿using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class StatisticsSummary
    {
        [Required]
        public string InstanceId { get; set; }

        public int TotalNumberOfTrades { get; set; }

        public int TotalNumberOfStarts { get; set; }

        public double InitialWalletBalance { get; set; }

        public double LastWalletBalance { get; set; }

        public double InitialTradedAssetBalance { get; set; }

        public double InitialAssetTwoBalance { get; set; }

        public double LastTradedAssetBalance { get; set; }

        public double LastAssetTwoBalance { get; set; }

        public string TradedAssetName { get; set; }

        public string AssetTwoName { get; set; }

        public string UserCurrencyBaseAssetId { get; set; }

        public double NetProfit { get; set; }
    }
}
