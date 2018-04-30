using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    /// <summary>
    /// <see cref="IStatisticsRepository"/> implementation
    /// </summary>
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly INoSQLTableStorage<StatisticsSummaryEntity> _tableSummary;

        public static readonly string TableName = "AlgoInstanceStatistics";

        public StatisticsRepository(
            INoSQLTableStorage<StatisticsSummaryEntity> tableSummary)
        {
            _tableSummary = tableSummary;
        }

        public static string GeneratePartitionKey(string instanceId) => instanceId;

        public static string GenerateSummaryRowKey() => "Summary";

        public async Task CreateAsync(StatisticsSummary summary)
        {
            var entitySummary = AutoMapper.Mapper.Map<StatisticsSummaryEntity>(summary);
            entitySummary.PartitionKey = GeneratePartitionKey(summary.InstanceId);
            entitySummary.RowKey = GenerateSummaryRowKey();

            await _tableSummary.InsertOrMergeAsync(entitySummary);
        }

        public async Task<StatisticsSummary> GetSummaryAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var rowKey = GenerateSummaryRowKey();

            var result = await _tableSummary.GetDataAsync(partitionKey, rowKey);

            return AutoMapper.Mapper.Map<StatisticsSummary>(result);
        }

        public async Task CreateOrUpdateSummaryAsync(StatisticsSummary data)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsSummaryEntity>(data);
            entity.PartitionKey = GeneratePartitionKey(data.InstanceId);
            entity.RowKey = GenerateSummaryRowKey();

            await _tableSummary.InsertOrMergeAsync(entity);
        }

        public async Task<bool> SummaryExistsAsync(string instanceId)
        {
            return await _tableSummary.RecordExistsAsync(new StatisticsSummaryEntity
            {
                PartitionKey = GeneratePartitionKey(instanceId),
                RowKey = GenerateSummaryRowKey()
            });
        }
    }
}
