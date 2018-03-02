using System.Collections.Generic;
using System.Linq;
using AzureStorage;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Newtonsoft.Json;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoClientInstanceRepository : IAlgoClientInstanceRepository
    {
        private readonly INoSQLTableStorage<AlgoClientInstanceEntity> _table;

        public static readonly string TableName = "AlgoClientInstanceTable";

        public AlgoClientInstanceRepository(INoSQLTableStorage<AlgoClientInstanceEntity> table)
        {
            _table = table;
        }

        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByAlgoAsync(string algoId)
        { 
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateAlgoIdPartitionKey(algoId));

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }

        public async Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByClientAsync(string clientId)
        {
            var entities = await _table.GetDataAsync(KeyGenerator.GenerateClientIdPartitionKey(clientId));

            var result = entities.Select(entity => entity.ToModel()).ToList();

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

        public async Task SaveAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var algoIdPartitionKeyEntity = data.ToEntityWithAlgoIdPartitionKey();
            var clientIdPartitionKeyEntity = data.ToEntityWithClientIdPartitionKey();

            await _table.InsertOrMergeAsync(algoIdPartitionKeyEntity);
            await _table.InsertOrMergeAsync(clientIdPartitionKeyEntity);
        }

        public async Task DeleteAlgoInstanceDataAsync(AlgoClientInstanceData data)
        {
            var algoIdPartitionKeyEntity = data.ToEntityWithAlgoIdPartitionKey();
            var clientIdPartitionKeyEntity = data.ToEntityWithClientIdPartitionKey();

            await _table.DeleteAsync(algoIdPartitionKeyEntity);
            await _table.DeleteAsync(clientIdPartitionKeyEntity);
        }

        public async Task<string> GetAlgoInstanceMetadataSetting(string algoId, string instanceId, string key)
        {
            var partitionKey = KeyGenerator.GenerateAlgoIdPartitionKey(algoId);
            var data = await _table.GetDataAsync(partitionKey,instanceId);
            if (data == null)
                return string.Empty;

            var algoMetadataInformation = JsonConvert.DeserializeObject<AlgoMetaDataInformation>(data.AlgoMetaDataInformation);           
            string settingValue = algoMetadataInformation.Parameters.Where(p => p.Key == key).Select(p => p.Value).SingleOrDefault();

            return settingValue ?? string.Empty;
        }
    }
}
