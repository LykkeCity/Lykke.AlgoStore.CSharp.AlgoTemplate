using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    /// <summary>
    /// Algo statistics
    /// </summary>
    public interface IStatistics
    {
        StatisticsSummary GetSummary();
    }
}
