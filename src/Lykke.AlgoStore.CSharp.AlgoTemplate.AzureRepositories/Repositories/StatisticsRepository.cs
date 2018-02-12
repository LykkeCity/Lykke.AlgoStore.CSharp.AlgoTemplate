﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entitites;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories
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

        public async Task<double> GetBoughtAmountAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBought);

            var result = data.Sum(x => x.Price);

            return result;
        }

        public async Task<double> GetSoldAmountAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => !x.IsBought);

            var result = data.Sum(x => x.Price);

            return result;
        }

        public async Task<double> GetBoughtQuantityAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => x.IsBought);

            var result = data.Sum(x => x.Amount);

            return result;
        }

        public async Task<double> GetSoldQuantityAsync(string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(partitionKey, x => !x.IsBought);

            var result = data.Sum(x => x.Amount);

            return result;
        }
    }
}
