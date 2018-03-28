using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Algo statistics
    /// </summary>
    public interface IStatistics
    {
        StatisticsSummary GetSummary();
    }
}
