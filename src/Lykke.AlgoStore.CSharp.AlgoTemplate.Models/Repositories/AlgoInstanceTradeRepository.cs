using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoInstanceTradeRepository : IAlgoInstanceTradeRepository
    {
        public static readonly string TableName = "AlgoInstanceTrades";

        private readonly object _sync = new object();
        private long _lastDifference = -1;
        private int _duplicateCounter = 99999;

        private readonly INoSQLTableStorage<AlgoInstanceTradeEntity> _tableStorage;

        public AlgoInstanceTradeRepository(INoSQLTableStorage<AlgoInstanceTradeEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        private static string GeneratePartitionKeyByOrderId(string orderId) => orderId;

        private static string GenerateRowKeyByWalletId(string walletId) => walletId;

        /// <summary>
        /// In order to get easily the trades of algo instance by asset we should have PK with instance Id and asset id 
        /// </summary>
        public static string GeneratePartitionKeyByInstanceIdAndAssetId(string instanceId, string assetId)
        {
            return instanceId + "_" + assetId;
        }

        public static string GenerateRowKey(long difference, int duplicateCounter) => String.Format("{0:D19}{1:D5}_{2}",
            difference, duplicateCounter, DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

        private string GenerateRowKey()
        {
            lock (_sync)
            {
                var difference = DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks;
                if (difference != _lastDifference)
                {
                    _lastDifference = difference;
                    _duplicateCounter = 99999;
                }
                else
                    _duplicateCounter -= 1;

                return GenerateRowKey(difference, _duplicateCounter);
            }
        }

        public async Task<AlgoInstanceTrade> GetAlgoInstanceOrderAsync(string orderId, string walletId)
        {
            var partitionKey = GeneratePartitionKeyByOrderId(orderId);
            var rowKey = GenerateRowKeyByWalletId(walletId);

            var result = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            if (result == null)
                return null;

            return AutoMapper.Mapper.Map<AlgoInstanceTrade>(result);
        }

        public async Task CreateOrUpdateAlgoInstanceOrderAsync(AlgoInstanceTrade data)
        {
            var entity = AutoMapper.Mapper.Map<AlgoInstanceTradeEntity>(data);
            entity.PartitionKey = GeneratePartitionKeyByOrderId(data.OrderId);
            entity.RowKey = GenerateRowKeyByWalletId(data.WalletId);

            await _tableStorage.InsertOrMergeAsync(entity);
        }

        public async Task CreateOrUpdateAlgoInstanceOrderAsync(AlgoInstanceTrade data)
        {
            var entity = AutoMapper.Mapper.Map<AlgoInstanceTradeEntity>(data);
            entity.PartitionKey = GeneratePartitionKeyByOrderId(data.OrderId);
            entity.RowKey = GenerateRowKeyByWalletId(data.WalletId);

            await _tableStorage.InsertOrMergeAsync(entity);
        }

        /// <summary>
        /// Create new trade row with PK - instance Id and RK - ReverseTick
        /// </summary>
        /// <param name="data"></param>
        public async Task SaveAlgoInstanceTradeAsync(AlgoInstanceTrade data)
        {
            var entity = AutoMapper.Mapper.Map<AlgoInstanceTradeEntity>(data);
            entity.PartitionKey = GeneratePartitionKeyByInstanceIdAndAssetId(data.InstanceId, data.AssetId);
            entity.RowKey = GenerateRowKey();

            await _tableStorage.InsertAsync(entity);
        }

        public async Task<IEnumerable<AlgoInstanceTrade>> GetAlgoInstaceTradesByTradedAssetAsync(string instanceId, string assetId, int maxNumberOfRowsToFetch)
        {
            var query = new TableQuery<AlgoInstanceTradeEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GeneratePartitionKeyByInstanceIdAndAssetId(instanceId, assetId)))
                .Take(maxNumberOfRowsToFetch);

            var result = new List<AlgoInstanceTrade>();

            await _tableStorage.ExecuteAsync(query,
                (items) => result.AddRange(AutoMapper.Mapper.Map<List<AlgoInstanceTrade>>(items)), () => false);

            return result;
        }

        public async Task<IEnumerable<AlgoInstanceTrade>> GetInstaceTradesByTradedAssetAndPeriodAsync(string instanceId, string assetId, DateTime from, DateTime to, CancellationToken ct)
        {
            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GeneratePartitionKeyByInstanceIdAndAssetId(instanceId, assetId));
            string fromFilter = TableQuery.GenerateFilterConditionForDate("DateOfTrade", QueryComparisons.GreaterThanOrEqual, from);
            string toFilter = TableQuery.GenerateFilterConditionForDate("DateOfTrade", QueryComparisons.LessThanOrEqual, to);

            var query = new TableQuery<AlgoInstanceTradeEntity>().Where(TableQuery.CombineFilters(TableQuery.CombineFilters(pkFilter, TableOperators.And, fromFilter), TableOperators.And, toFilter));

            var result = new List<AlgoInstanceTrade>();

            await _tableStorage.GetDataByChunksAsync(query, (items) => result.AddRange(AutoMapper.Mapper.Map<List<AlgoInstanceTrade>>(items))).ContinueWith(t => { }, ct);

            return result;
        }
    }
}
