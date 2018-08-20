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
            var result = await _table.GetDataAsync(r => r.RowKey == clientId);
            return result.ToList().ToModel();
        }

        public async Task<IList<AlgoRatingData>> GetAlgoRatingsAsync(string algoId)
        {
            var result = await _table.GetDataAsync(algoId);
            return result.ToList().ToModel();
        }

        public async Task SaveAlgoRatingAsync(AlgoRatingData data)
        {
            var entities = data.ToEntity();
            await _table.InsertOrReplaceAsync(entities);
        }

        public async Task SaveAlgoRatingWithFakeIdAsync(AlgoRatingData data)
        {
            await _table.DeleteIfExistAsync(data.AlgoId, data.ClientId);

            data.ClientId = _deactivatedFakeClientId;
            var entities = data.ToEntity();

            await _table.InsertOrReplaceAsync(entities);
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
                }

                await _table.DoBatchAsync(tableBatchOperation);
            }, () => true);
        }
    }

}
