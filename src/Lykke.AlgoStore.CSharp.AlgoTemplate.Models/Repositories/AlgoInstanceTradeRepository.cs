﻿using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static string GenerateRowKeyByOrderId(string walletId) => walletId;

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
            var rowKey = GenerateRowKeyByOrderId(walletId);

            var result = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            if (result == null)
                return null;

            return AutoMapper.Mapper.Map<AlgoInstanceTrade>(result);
        }

        public async Task CreateAlgoInstanceOrderAsync(AlgoInstanceTrade data)
        {
            var entity = AutoMapper.Mapper.Map<AlgoInstanceTradeEntity>(data);
            entity.PartitionKey = GeneratePartitionKeyByOrderId(data.OrderId);
            entity.RowKey = GenerateRowKeyByOrderId(data.WalletId);

            await _tableStorage.InsertAsync(entity);
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

        public async Task<IEnumerable<AlgoInstanceTrade>> GetAlgoInstaceTradesByTradedAssetAsync(string instanceId, string assetId, int maxNumberOfRowsToFetch = 0)
        {
            var partition = GeneratePartitionKeyByInstanceIdAndAssetId(instanceId, assetId);

            var data = await _tableStorage.GetDataAsync(partition, x => x.IsBuy.HasValue);

            if (maxNumberOfRowsToFetch > 0)
                data = data.Take(maxNumberOfRowsToFetch);

            return AutoMapper.Mapper.Map<List<AlgoInstanceTrade>>(data);
        }
    }
}
