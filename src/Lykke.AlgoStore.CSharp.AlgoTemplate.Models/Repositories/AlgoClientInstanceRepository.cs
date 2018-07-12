using System;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoClientInstanceRepository : IAlgoClientInstanceRepository
    {
        private readonly INoSQLTableStorage<AlgoClientInstanceEntity> _table;
        private readonly INoSQLTableStorage<AlgoInstanceStoppingEntity> _stoppingEntityTable;
        private readonly INoSQLTableStorage<AlgoInstanceTcBuildEntity> _tcBuildEntityTable;

        public static readonly string TableName = "AlgoClientInstanceTable";

        public AlgoClientInstanceRepository(INoSQLTableStorage<AlgoClientInstanceEntity> table,
            INoSQLTableStorage<AlgoInstanceStoppingEntity> stoppingEntityTable,
            INoSQLTableStorage<AlgoInstanceTcBuildEntity> tcBuildEntityTable)
        {
            _table = table;
            _stoppingEntityTable = stoppingEntityTable;
            _tcBuildEntityTable = tcBuildEntityTable;
        }

        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByAlgoAsync(string algoId)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateAlgoIdPartitionKey(algoId));

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }

        public async Task<IEnumerable<AlgoClientInstanceData>> GetAllAlgoInstancesByAlgoIdAndClienIdAsync(string algoId, string clientId)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateAlgoIdAndClientIdPartitionKey(algoId, clientId));
            var result = entities.Select(entity => entity.ToModel());
            return result;
        }

        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByClientAsync(string clientId)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateClientIdPartitionKey(clientId));

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }

        public async Task<IEnumerable<AlgoClientInstanceData>> GetAllAlgoInstancesByAlgoIdAndInstanceTypeAsync(string algoId, AlgoInstanceType instanceType)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateAlgoIdAndInstanceTypePartitionKey(algoId, instanceType));

            var result = entities.Select(entity => entity.ToModel());

            return result;
        }

        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataByAlgoIdAsync(string algoId, string instanceId)
        {
            var entitiy = await _table.GetDataAsync(KeyGenerator.GenerateAlgoIdPartitionKey(algoId), instanceId);

            return entitiy.ToModel();
        }

        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataByClientIdAsync(string clientId, string instanceId)
        {
            var entitiy = await _table.GetDataAsync(KeyGenerator.GenerateClientIdPartitionKey(clientId), instanceId);

            return entitiy.ToModel();
        }

        public async Task<AlgoClientInstanceData> GetAlgoInstanceDataByAuthTokenAsync(string authToken)
        {
            var entity = await _table.GetDataAsync(KeyGenerator.GenerateAuthTokenPartitionKey(authToken));

            return entity.FirstOrDefault()?.ToModel();
        }

        public async Task<AlgoInstanceTcBuildData> GetAlgoInstanceDataByTcBuildIdAsync(string tcBuildId)
        {
            var entity = await _tcBuildEntityTable.GetDataAsync(KeyGenerator.GenerateTcBuildEntityPartitionKey(),
                tcBuildId);

            return AutoMapper.Mapper.Map<AlgoInstanceTcBuildData>(entity);
        }

        public async Task<IEnumerable<AlgoInstanceStoppingData>> GetAllAlgoInstancesPastEndDate(DateTime dateToCheck)
        {
            var entities = await _stoppingEntityTable.GetDataAsync(KeyGenerator.GenerateStoppingEntityPartitionKey(),
                e => long.Parse(e.RowKey) <= dateToCheck.Ticks);

            var result = entities.Select(AutoMapper.Mapper.Map<AlgoInstanceStoppingData>);

            return result;
        }

        public async Task<IEnumerable<AlgoClientInstanceData>> GetAllByWalletIdAndInstanceStatusIsNotStoppedAsync(string walletId)
        {
            var entities = (await _table.GetDataAsync(KeyGenerator.GenerateWalletIdPartitionKey(walletId)))
                                    .Where(a => a.AlgoInstanceStatus != AlgoInstanceStatus.Stopped);
            var result = entities.Select(entity => entity.ToModel());
            return result;
        }

        public async Task<bool> ExistsAlgoInstanceDataWithAlgoIdAsync(string algoId, string instanceId)
        {
            var entity = new AlgoClientInstanceEntity
            {
                PartitionKey = KeyGenerator.GenerateAlgoIdPartitionKey(algoId),
                RowKey = instanceId
            };

            return await _table.RecordExistsAsync(entity);
        }

        public async Task<bool> ExistsAlgoInstanceDataWithClientIdAsync(string clientId, string instanceId)
        {
            var entity = new AlgoClientInstanceEntity
            {
                PartitionKey = KeyGenerator.GenerateClientIdPartitionKey(clientId),
                RowKey = instanceId
            };

            return await _table.RecordExistsAsync(entity);
        }

        public async Task<bool> ExistsAlgoInstanceDataWithAuthTokenAsync(string authToken)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateAuthTokenPartitionKey(authToken));

            return entities.Any();
        }

        public async Task<bool> ExistsAlgoInstanceDataWithTcBuildIdAsync(string tcBuildId)
        {
            var entity = await _tcBuildEntityTable.GetDataAsync(KeyGenerator.GenerateTcBuildEntityPartitionKey(),
                tcBuildId);

            return await _tcBuildEntityTable.RecordExistsAsync(entity);
        }

        public async Task SaveAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var algoIdPartitionKeyEntity = data.ToEntityWithAlgoIdPartitionKey();
            var clientIdPartitionKeyEntity = data.ToEntityWithClientIdPartitionKey();
            var algoIdAndClientIdPartitionKeyEntity = data.ToEntityWithAlgoIdAndClientIdPartitionKey();
            var algoIdAndInstanceTypePartitionKeyEntity = data.ToEntityWithAlgoIdAndInstanceTypePartitionKey();
            var authTokenPartitionKeyEntity = data.ToEntityWithAuthTokenPartitionKey();        
            AlgoInstanceStoppingEntity endDatePartitionKeyEntity;

            if (!string.IsNullOrEmpty(data.WalletId))
            {
                var walletIdPartitionKeyEntity = data.ToEntityWithWalletIdPartitionKey();
                await _table.InsertOrMergeAsync(walletIdPartitionKeyEntity);
            }

            await _table.InsertOrMergeAsync(algoIdPartitionKeyEntity);
            await _table.InsertOrMergeAsync(clientIdPartitionKeyEntity);
            await _table.InsertOrMergeAsync(algoIdAndClientIdPartitionKeyEntity);
            await _table.InsertOrMergeAsync(algoIdAndInstanceTypePartitionKeyEntity);
            await _table.InsertOrMergeAsync(authTokenPartitionKeyEntity);

            if (data.AlgoInstanceType != AlgoInstanceType.Test && data.AlgoInstanceStatus == AlgoInstanceStatus.Stopped)
            {
                endDatePartitionKeyEntity = data.ToStoppingEntityWithEndDatePartitionKey();
                await _stoppingEntityTable.DeleteIfExistAsync(endDatePartitionKeyEntity.PartitionKey, endDatePartitionKeyEntity.RowKey);
            }
            else if (data.AlgoInstanceType != AlgoInstanceType.Test && data.AlgoInstanceStatus != AlgoInstanceStatus.Stopped)
            {
                endDatePartitionKeyEntity = data.ToStoppingEntityWithEndDatePartitionKey();
                await _stoppingEntityTable.InsertOrMergeAsync(endDatePartitionKeyEntity);
            }         

            if (!string.IsNullOrEmpty(data.TcBuildId))
            {
                var teamCityBuilIdPartitionKeyEntity = data.ToEntityWithTcBuildIdPartitionKey();
                await _tcBuildEntityTable.InsertOrMergeAsync(teamCityBuilIdPartitionKeyEntity);
            }
        }

        public async Task DeleteAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var algoIdPartitionKeyEntity = data.ToEntityWithAlgoIdPartitionKey();
            var clientIdPartitionKeyEntity = data.ToEntityWithClientIdPartitionKey();
            var algoIdAndClientIdPartitionKeyEntity = data.ToEntityWithAlgoIdAndClientIdPartitionKey();
            var algoIdAndInstanceTypePartitionKeyEntity = data.ToEntityWithAlgoIdAndInstanceTypePartitionKey();
            var authTokenPartitionKeyEntity = data.ToEntityWithAuthTokenPartitionKey();
            var endDatePartitionKeyEntity = data.ToStoppingEntityWithEndDatePartitionKey();          

            if (!string.IsNullOrEmpty(data.WalletId))
            {
                var walletIdPartitionKeyEntity = data.ToEntityWithWalletIdPartitionKey();
                await _table.DeleteIfExistAsync(walletIdPartitionKeyEntity.PartitionKey, walletIdPartitionKeyEntity.RowKey);
            }

            await _table.DeleteIfExistAsync(algoIdPartitionKeyEntity.PartitionKey, algoIdPartitionKeyEntity.RowKey);
            await _table.DeleteIfExistAsync(clientIdPartitionKeyEntity.PartitionKey, clientIdPartitionKeyEntity.RowKey);
            await _table.DeleteIfExistAsync(algoIdAndClientIdPartitionKeyEntity.PartitionKey, algoIdAndClientIdPartitionKeyEntity.RowKey);
            await _table.DeleteIfExistAsync(algoIdAndInstanceTypePartitionKeyEntity.PartitionKey, algoIdAndClientIdPartitionKeyEntity.RowKey);
            await _table.DeleteIfExistAsync(authTokenPartitionKeyEntity.PartitionKey, authTokenPartitionKeyEntity.RowKey);
            await _stoppingEntityTable.DeleteIfExistAsync(endDatePartitionKeyEntity.PartitionKey, endDatePartitionKeyEntity.RowKey);

            if (!string.IsNullOrEmpty(data.TcBuildId))
            {
                var teamCityBuilIdPartitionKeyEntity = data.ToEntityWithTcBuildIdPartitionKey();
                await _tcBuildEntityTable.DeleteIfExistAsync(teamCityBuilIdPartitionKeyEntity.PartitionKey,
                    teamCityBuilIdPartitionKeyEntity.RowKey);
            }            
        }

        public async Task<string> GetAlgoInstanceMetadataSetting(string algoId, string instanceId, string key)
        {
            var partitionKey = KeyGenerator.GenerateAlgoIdPartitionKey(algoId);
            var data = await _table.GetDataAsync(partitionKey, instanceId);
            if (data == null)
                return string.Empty;

            var algoMetadataInformation = JsonConvert.DeserializeObject<AlgoMetaDataInformation>(data.AlgoMetaDataInformation);
            string settingValue = algoMetadataInformation.Parameters.Where(p => p.Key == key).Select(p => p.Value).SingleOrDefault();

            return settingValue ?? string.Empty;
        }

        public async Task<bool> HasInstanceData(string clientId, string algoId)
        {
            string keyWithAlgoId = KeyGenerator.GenerateAlgoIdPartitionKey(algoId);
            string keyWithClientId = KeyGenerator.GenerateClientIdPartitionKey(clientId);
            string keyWithAlgoIdAndClientId = KeyGenerator.GenerateAlgoIdAndClientIdPartitionKey(algoId, clientId);

            var dataWithAlgoIdPartitionKey = await _table.GetTopRecordAsync(keyWithAlgoId);
            var dataWithClientIdPartitionKey = await _table.GetTopRecordAsync(keyWithClientId);
            var dataWithAlgoIdAndClientIdPartitionKey = await _table.GetTopRecordAsync(keyWithAlgoIdAndClientId);

            return dataWithAlgoIdPartitionKey != null || dataWithClientIdPartitionKey != null
                                || dataWithAlgoIdAndClientIdPartitionKey != null;
        }
    }
}
