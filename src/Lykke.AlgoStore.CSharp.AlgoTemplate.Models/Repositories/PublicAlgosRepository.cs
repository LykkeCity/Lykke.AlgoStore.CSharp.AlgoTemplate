using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class PublicAlgosRepository : IPublicAlgosRepository
    {
        public static readonly string TableName = "PublicAlgosTable";

        private readonly string _deactivatedFakeClientId = "Deactivated";

        private readonly INoSQLTableStorage<PublicAlgoEntity> _table;

        public PublicAlgosRepository(INoSQLTableStorage<PublicAlgoEntity> table)
        {
            _table = table;
        }

        public async Task<List<PublicAlgoData>> GetAllPublicAlgosAsync()
        {
            var entities = await _table.GetDataAsync(PublicAlgoMapper.PartitionKey);

            var result = entities.Select(entity => entity.ToModel()).ToList();

            return result;
        }
        public async Task<bool> ExistsPublicAlgoAsync(string clientId, string algoId)
        {
            var entity = new PublicAlgoEntity
            {
                PartitionKey = PublicAlgoMapper.PartitionKey,
                RowKey = KeyGenerator.GenerateKey(clientId, algoId)
            };

            return await _table.RecordExistsAsync(entity);
        }

        public async Task SavePublicAlgoAsync(PublicAlgoData data)
        {
            var enitites = data.ToEntity();

            await _table.InsertOrMergeAsync(enitites);
        }

        public async Task SavePublicAlgoNewPKAsync(PublicAlgoData data)
        {
            var algoEntity = data.ToEntity();

            await _table.DeleteIfExistAsync(PublicAlgoMapper.PartitionKey, KeyGenerator.GenerateKey(data.ClientId, data.AlgoId););

            data.ClientId = _deactivatedFakeClientId;
            var fakeAlgoEntity = data.ToEntity();

            await _table.InsertOrMergeAsync(fakeAlgoEntity);
        }

        public async Task DeletePublicAlgoAsync(PublicAlgoData data)
        {
            var entities = data.ToEntity();
            await _table.DeleteAsync(entities);
        }
    }
}
