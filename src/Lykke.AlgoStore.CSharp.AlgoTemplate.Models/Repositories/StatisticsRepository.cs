using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    /// <summary>
    /// <see cref="IStatisticsRepository"/> implementation
    /// </summary>
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly INoSQLTableStorage<StatisticsEntity> _table;
        private readonly INoSQLTableStorage<StatisticsSummaryEntity> _tableSummary;

        public static readonly string TableName = "AlgoInstanceStatistics";

        public StatisticsRepository(
            INoSQLTableStorage<StatisticsEntity> table,
            INoSQLTableStorage<StatisticsSummaryEntity> tableSummary)
        {
            _table = table;
            _tableSummary = tableSummary;
        }

        public static string GeneratePartitionKey(string instanceId) => instanceId;

        public static string GenerateRowKey(string key) => String.IsNullOrEmpty(key) ? DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : key;

        public static string GenerateSummaryRowKey() => "Summary";

        public async Task CreateAsync(Statistics data)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsEntity>(data);
            entity.PartitionKey = GeneratePartitionKey(data.InstanceId);
            entity.RowKey = GenerateRowKey(data.Id);

            await _table.InsertAsync(entity);
        }

        public async Task CreateAsync(Statistics data, StatisticsSummary summary)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsEntity>(data);
            entity.PartitionKey = GeneratePartitionKey(data.InstanceId);
            entity.RowKey = GenerateRowKey(data.Id);

            var entitySummary = AutoMapper.Mapper.Map<StatisticsSummaryEntity>(summary);
            entitySummary.PartitionKey = GeneratePartitionKey(summary.InstanceId);
            entitySummary.RowKey = GenerateSummaryRowKey();

            var batch = new TableBatchOperation();
            batch.Insert(entity);
            batch.InsertOrMerge(entitySummary);

            await _table.DoBatchAsync(batch);
        }

        public async Task DeleteAsync(string instanceId, string id)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var rowKey = id;

            await _table.DeleteAsync(partitionKey, rowKey);
        }

        public async Task DeleteAllAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);

            //REMARK: This is potential problem due to large amount of data that can have same partition key
            //Maybe we should reconsider and have another approach and have one table per algo instance
            //In that way we can delete complete table
            var dataToDelete = await _table.GetDataAsync(partitionKey);

            await _table.DeleteAsync(dataToDelete);
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

        public async Task<List<Statistics>> GetAllStatisticsAsync(string instanceId, int maxNumberOfRowsToFetch = 0)
        {
            IEnumerable<StatisticsEntity> data;

            if (maxNumberOfRowsToFetch <= 0)
                data = await _table.GetDataAsync(GeneratePartitionKey(instanceId));
            else
                data = await _table.GetTopRecordsAsync(GeneratePartitionKey(instanceId), maxNumberOfRowsToFetch);

            return AutoMapper.Mapper.Map<List<Statistics>>(data);
        }
    }
}
