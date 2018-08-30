using System.Collections.Generic;
using AzureStorage;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoRepository : IAlgoRepository
    {
        public static readonly string TableName = "Algos";

        private readonly INoSQLTableStorage<AlgoEntity> _table;

        public AlgoRepository(INoSQLTableStorage<AlgoEntity> table)
        {
            _table = table;
        }

        public async Task<IEnumerable<IAlgo>> GetAllAlgosAsync()
        {
            var result = await _table.GetDataAsync();
            return result.ToList();
        }

        public async Task<IEnumerable<IAlgo>> GetAllClientAlgosAsync(string clientId)
        {
            var entities = await _table.GetDataAsync(clientId);
            return entities.OrderBy(a => a.Name);
        }

        public async Task<IAlgo> GetAlgoByAlgoIdAsync(string algoId)
        {
            var entity = (await _table.GetDataAsync(algoId));
            return entity.FirstOrDefault();
        }

        public async Task<bool> IsExistingAlgoIdForAnotherUserAsync(string algoId, string clientId)
        {
            var entity = (await _table.GetDataAsync(algoId));
            return entity.Any(a => a.ClientId != clientId);
        }

        public async Task<IAlgo> GetAlgoAsync(string clientId, string algoId)
        {
            var entity = await _table.GetDataAsync(clientId, algoId);

            return entity;
        }

        public async Task<AlgoDataInformation> GetAlgoDataInformationAsync(string algoId)
        {
            var entitiy = await _table.GetDataAsync(algoId);

            return entitiy.FirstOrDefault()?.ToAlgoDataInformation();
        }

        public async Task<bool> ExistsAlgoAsync(string clientId, string algoId)
        {
            var entity = new AlgoEntity
            {
                PartitionKey = clientId,
                RowKey = algoId
            };

            return await _table.RecordExistsAsync(entity);
        }

        public async Task<bool> ExistsAlgoAsync(string algoId)
        {
            var entity = await _table.GetDataAsync(algoId);
            return entity.Any();
        }

        public async Task SaveAlgoAsync(IAlgo algo)
        {
            var enitity = AlgoEntity.Create(algo);
            await _table.InsertOrReplaceAsync(enitity);

            var algoIdPartitionKeyEntity = AlgoEntity.CreateEntityWithAlgoIdPartionKey(algo);
            await _table.InsertOrReplaceAsync(algoIdPartitionKeyEntity);
        }

        public async Task SaveAlgoWithNewPKAsync(IAlgo algo, string oldPK)
        {
            var enitity = AlgoEntity.Create(algo);

            await _table.DeleteIfExistAsync(oldPK, enitity.RowKey);

            await _table.InsertOrMergeAsync(enitity);

            var algoIdPartitionKeyEntity = AlgoEntity.CreateEntityWithAlgoIdPartionKey(algo);

            await _table.DeleteIfExistAsync(algoIdPartitionKeyEntity.PartitionKey, oldPK);

            await _table.InsertOrMergeAsync(algoIdPartitionKeyEntity);
        }

        public async Task DeleteAlgoAsync(string clientId, string algoId)
        {
            await _table.DeleteAsync(clientId, algoId);
            await _table.DeleteAsync(algoId, clientId);
        }
    }
}
