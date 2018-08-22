using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IStatisticsRepository
    {
        Task CreateAsync(StatisticsSummary summary);
        Task<StatisticsSummary> GetSummaryAsync(string instanceId);
        Task CreateOrUpdateSummaryAsync(StatisticsSummary data);
        Task<bool> SummaryExistsAsync(string instanceId);
        Task DeleteSummaryAsync(string instanceId);
    }
}
