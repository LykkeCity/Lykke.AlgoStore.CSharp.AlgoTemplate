using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoRatingsRepository : IAlgoRatingsRepository
    {
        public static readonly string TableName = "AlgoRatingsTable";
        private readonly string _deactivatedFakeClientId = "Deactivated";

        private readonly INoSQLTableStorage<AlgoRatingEntity> _table;

        public AlgoRatingsRepository(INoSQLTableStorage<AlgoRatingEntity> table)
        {
            _table = table;
        }

        public async Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId)
        {
            var result = await _table.GetDataAsync(algoId, clientId);
            return result.ToModel();
        }

        public async Task<IList<AlgoRatingData>> GetAlgoRatingsByClientIdAsync(string clientId)
        {
            var result = await _table.GetDataAsync(clientId);
            return result.ToList().ToModelWithPrimaryKeyClientId();
        }

        public async Task<IList<AlgoRatingData>> GetAlgoRatingsAsync(string algoId)
        {
            var result = await _table.GetDataAsync(algoId);
            return result.ToList().ToModel();
        }

        public async Task SaveAlgoRatingAsync(AlgoRatingData data)
        {
            var entityWithAlgoIdPK = data.ToEntity();
            var entityWithClientIdPK = data.ToEntityWithClientIdPartitionKey();

            await _table.InsertOrReplaceAsync(entityWithAlgoIdPK);
            await _table.InsertOrReplaceAsync(entityWithClientIdPK);
        }

        public async Task SaveAlgoRatingWithFakeIdAsync(AlgoRatingData data)
        {
            await _table.DeleteIfExistAsync(data.AlgoId, data.ClientId);
            await _table.DeleteIfExistAsync(data.ClientId, data.AlgoId);

            data.ClientId = _deactivatedFakeClientId;
            var entityWithAlgoIdPK = data.ToEntity();
            var entityWithClientIdPK = data.ToEntityWithClientIdPartitionKey();

            await _table.InsertOrReplaceAsync(entityWithAlgoIdPK);
            await _table.InsertOrReplaceAsync(entityWithClientIdPK);
        }

        public async Task DeleteRatingsAsync(string algoId)
        {
            var query = new TableQuery<AlgoRatingEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, algoId))
                .Take(100);

            await _table.ExecuteAsync(query, async (ratings) =>
            {
                var tableBatchOperation = new TableBatchOperation();

                foreach (var rating in ratings)
                {
                    tableBatchOperation.Delete(rating);

                    //delete the algo rating with PrimariKey client id and Row Key algo id
                    await _table.DeleteIfExistAsync(rating.RowKey, rating.PartitionKey);
                }

                await _table.DoBatchAsync(tableBatchOperation);
            }, () => true);
        }
    }
}
