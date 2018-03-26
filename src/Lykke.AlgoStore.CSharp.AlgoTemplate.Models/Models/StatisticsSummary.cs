using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class StatisticsSummary
    {
        [Required]
        public string InstanceId { get; set; }

        [Required]
        public AlgoInstanceType InstanceType { get; set; }

        public int TotalNumberOfTrades { get; set; }

        public int TotalNumberOfStarts { get; set; }

        public decimal InitialWalletBalance { get; set; }

        public decimal LastWalletBalance { get; set; }

        public decimal AssetOneBalance { get; set; }

        public decimal AssetTwoBalance { get; set; }
    }
}
