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
    }
}
