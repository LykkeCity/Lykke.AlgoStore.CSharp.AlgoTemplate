using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class StatisticsSummary
    {
        public string InstanceId { get; set; }

        public string Id { get; set; }

        public AlgoInstanceType InstanceType { get; set; }

        public int TotalNumberOfTrades { get; set; }

        public int TotalNumberOfStarts { get; set; }
    }
}
