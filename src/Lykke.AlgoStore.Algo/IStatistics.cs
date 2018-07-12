using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Algo
{
    /// <summary>
    /// Algo statistics
    /// </summary>
    public interface IStatistics
    {
        StatisticsSummary GetSummary();
    }
}
