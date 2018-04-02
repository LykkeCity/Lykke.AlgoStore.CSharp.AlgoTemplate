using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IStatisticsRepository
    {
        Task CreateAsync(Statistics data);
        Task CreateAsync(Statistics data, StatisticsSummary summary);
        Task DeleteAsync(string instanceId, string id);
        Task DeleteAllAsync(string instanceId);
        Task<StatisticsSummary> GetSummaryAsync(string instanceId);
        Task CreateOrUpdateSummaryAsync(StatisticsSummary data);
        Task<bool> SummaryExistsAsync(string instanceId);
        Task<List<Statistics>> GetAllStatisticsAsync(string instanceId, int maxNumberOfRowsToFetch = 0);
    }
}
