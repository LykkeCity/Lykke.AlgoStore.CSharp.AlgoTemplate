﻿using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

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

        public double AssetOneBalance { get; set; }

        public double AssetTwoBalance { get; set; }

        public string UserCurrencyBaseAssetId { get; set; }

        public double NetProfit { get; set; }
    }
}
