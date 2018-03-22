using System;
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

        public static readonly string TableName = "AlgoStatistics";
        public static readonly string SummaryTableName = "AlgoStatisticsSummary";

        public StatisticsRepository(
            INoSQLTableStorage<StatisticsEntity> table, 
            INoSQLTableStorage<StatisticsSummaryEntity> tableSummary)
        {
            _table = table;
            _tableSummary = tableSummary;
        }

        public static string GeneratePartitionKey(string instanceId, AlgoInstanceType instanceType) => $"{instanceId}_{instanceType}";

        public static string GenerateRowKey(string key) => String.IsNullOrEmpty(key) ? DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : key;

        public static string GenerateSummaryRowKey() => "Summary";

        public async Task CreateAsync(Statistics data)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsEntity>(data);
            entity.PartitionKey = GeneratePartitionKey(data.InstanceId, data.InstanceType);
            entity.RowKey = GenerateRowKey(data.Id);

            var entitySummary = await _tableSummary.GetDataAsync(
                GeneratePartitionKey(data.InstanceId, data.InstanceType),
                GenerateSummaryRowKey());

            if (entitySummary == null)
            {
                entitySummary = AutoMapper.Mapper.Map<StatisticsSummaryEntity>(data);
                entitySummary.PartitionKey = GeneratePartitionKey(data.InstanceId, data.InstanceType);
                entitySummary.RowKey = GenerateSummaryRowKey();
            }

            entitySummary.TotalNumberOfTrades++;

            var batch = new TableBatchOperation();
            batch.Insert(entity);
            batch.InsertOrMerge(entitySummary);

            await _table.DoBatchAsync(batch);
        }

        public async Task DeleteAsync(string instanceId, AlgoInstanceType instanceType, string id)
        {
            var partitionKey = GeneratePartitionKey(instanceId, instanceType);
            var rowKey = id;

            await _table.DeleteAsync(partitionKey, rowKey);
        }

        public async Task DeleteAllAsync(string instanceId, AlgoInstanceType instanceType)
        {
            var partitionKey = GeneratePartitionKey(instanceId, instanceType);

            //REMARK: This is potential problem due to large amount of data that can have same partition key
            //Maybe we should reconsider and have another approach and have one table per algo instance
            //In that way we can delete complete table
            var dataToDelete = await _table.GetDataAsync(partitionKey);

            await _table.DeleteAsync(dataToDelete);
        }
    }
}
