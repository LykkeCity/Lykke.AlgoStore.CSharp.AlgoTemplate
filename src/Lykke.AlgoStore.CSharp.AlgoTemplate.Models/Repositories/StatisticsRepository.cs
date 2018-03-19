using System;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    /// <summary>
    /// <see cref="IStatisticsRepository"/> implementation
    /// </summary>
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly INoSQLTableStorage<StatisticsEntity> _table;

        public static readonly string TableName = "Statistics";

        public StatisticsRepository(INoSQLTableStorage<StatisticsEntity> table)
        {
            _table = table;
        }

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(string key) => String.IsNullOrEmpty(key) ? DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : key;

        public async Task CreateAsync(Statistics data)
        {
            var entity = AutoMapper.Mapper.Map<StatisticsEntity>(data);

            entity.PartitionKey = GeneratePartitionKey(data.InstanceId);
            entity.RowKey = GenerateRowKey(data.Id);

            await _table.InsertAsync(entity);
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

        public async Task<double> GetBoughtAmountAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBuy.HasValue && x.IsBuy.Value && x.Price.HasValue);

            var result = data.Sum(x => x.Price);

            return result ?? 0;
        }

        public async Task<double> GetSoldAmountAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBuy.HasValue && !x.IsBuy.Value);

            var result = data.Sum(x => x.Price);

            return result ?? 0;
        }

        public async Task<double> GetBoughtQuantityAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBuy.HasValue && x.IsBuy.Value);

            var result = data.Sum(x => x.Amount);

            return result ?? 0;
        }

        public async Task<double> GetSoldQuantityAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBuy.HasValue && !x.IsBuy.Value);

            var result = data.Sum(x => x.Amount);

            return result ?? 0;
        }

        public async Task<int> GetNumberOfRunnings(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsStarted.HasValue && x.IsStarted.Value);

            var result = data.Count();

            return result;
        }
    }
}
