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
                throw new ArgumentException("Statistics summary row not found");

            if (data.IsStarted.HasValue && data.IsStarted.Value)
                entitySummary.TotalNumberOfStarts++;

            if (data.IsBuy.HasValue)
            {
                entitySummary.TotalNumberOfTrades++;

                if(!entity.Amount.HasValue)
                    throw new ArgumentException("Amount for statistics not provided");

                if (!entity.Price.HasValue)
                    throw new ArgumentException("Price for statistics not provided");

                if (data.IsBuy.Value)
                {
                    entitySummary.AssetOneBalance = entitySummary.AssetOneBalance + entity.Amount.Value;
                    entitySummary.AssetTwoBalance = entitySummary.AssetTwoBalance - (entity.Amount.Value * entity.Price.Value);
                    entitySummary.LastWalletBalance = entitySummary.AssetOneBalance + entitySummary.AssetTwoBalance;
                }
                else
                {
                    entitySummary.AssetOneBalance = entitySummary.AssetOneBalance - entity.Amount.Value;
                    entitySummary.AssetTwoBalance = entitySummary.AssetTwoBalance + (entity.Amount.Value * entity.Price.Value);
                    entitySummary.LastWalletBalance = entitySummary.AssetOneBalance + entitySummary.AssetTwoBalance;
                }
            }

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

        public async Task<StatisticsSummary> GetSummaryAsync(string instanceId, AlgoInstanceType instanceType)
        {
            var partitionKey = GeneratePartitionKey(instanceId, instanceType);
            var rowKey = GenerateSummaryRowKey();

            var result = await _tableSummary.GetDataAsync(partitionKey, rowKey);

            return AutoMapper.Mapper.Map<StatisticsSummary>(result);
        }

        public async Task CreateSummaryAsync(StatisticsSummary data)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsSummaryEntity>(data);
            entity.PartitionKey = GeneratePartitionKey(data.InstanceId, data.InstanceType);
            entity.RowKey = GenerateSummaryRowKey();

            //If this is not first time algo instance is started
            //we need to update existing statistics summary
            var existingEntitySummary =
                await _tableSummary.GetDataAsync(GeneratePartitionKey(data.InstanceId, data.InstanceType),
                    GenerateSummaryRowKey());

            if(existingEntitySummary == null)
            {
                await _tableSummary.InsertAsync(entity);
                return;
            }

            existingEntitySummary.AssetOneBalance = entity.AssetOneBalance;
            existingEntitySummary.AssetTwoBalance = entity.AssetTwoBalance;
            existingEntitySummary.InitialWalletBalance = entity.InitialWalletBalance;
            existingEntitySummary.LastWalletBalance = entity.LastWalletBalance;

            await _tableSummary.InsertOrMergeAsync(existingEntitySummary);
        }
    }
}
