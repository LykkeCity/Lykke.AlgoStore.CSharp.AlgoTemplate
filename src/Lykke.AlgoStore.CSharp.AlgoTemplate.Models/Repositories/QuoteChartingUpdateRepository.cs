using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class QuoteChartingUpdateRepository : IQuoteChartingUpdateRepository
    {
        private readonly INoSQLTableStorage<QuoteChartingUpdateEntity> _table;
        public static readonly string TableName = "AlgoInstanceQuotesChartingTable";

        private readonly object _sync = new object();
        private long _lastDifference = -1;
        private int _duplicateCounter = 99999;

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(long difference, int duplicateCounter) =>
            string.Format("{0:D19}{1:D5}_{2}", difference, duplicateCounter, Guid.NewGuid());

        public QuoteChartingUpdateRepository(INoSQLTableStorage<QuoteChartingUpdateEntity> table)
        {
            _table = table;
        }

        public async Task WriteAsync(IEnumerable<QuoteChartingUpdateData> data)
        {
            var batch = new TableBatchOperation();

            foreach (var chartingUpdate in data)
            {
                var entity = AutoMapper.Mapper.Map<QuoteChartingUpdateEntity>(chartingUpdate);

                entity.PartitionKey = GeneratePartitionKey(chartingUpdate.InstanceId);
                entity.RowKey = GenerateRowKey();

                batch.Insert(entity);
            }

            await _table.DoBatchAsync(batch);
        }

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

        public async Task<IEnumerable<QuoteChartingUpdateData>> GetQuotesForPeriodAsync(string instanceId, string assetPair, DateTime @from, DateTime to, CancellationToken ct, bool? isBuy = null)
        {
            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GeneratePartitionKey(instanceId));
            string fromFilter = TableQuery.GenerateFilterConditionForDate("QuoteTimestamp", QueryComparisons.GreaterThanOrEqual, from);
            string toFilter = TableQuery.GenerateFilterConditionForDate("QuoteTimestamp", QueryComparisons.LessThanOrEqual, to);
            string assetPairFilter = TableQuery.GenerateFilterCondition("AssetPair", QueryComparisons.Equal, assetPair);
            
            var filter = TableQuery.CombineFilters(TableQuery.CombineFilters(pkFilter, TableOperators.And, fromFilter), TableOperators.And, toFilter);
            filter = TableQuery.CombineFilters(filter, TableOperators.And, assetPairFilter);

            if (isBuy.HasValue)
            {
                string isBuyFilter = TableQuery.GenerateFilterConditionForBool("IsBuy", QueryComparisons.Equal, isBuy.Value);
                filter = TableQuery.CombineFilters(filter, TableOperators.And, isBuyFilter);
            }

            var query = new TableQuery<QuoteChartingUpdateEntity>().Where(filter);

            var result = new List<QuoteChartingUpdateData>();

            await _table.GetDataByChunksAsync(query, (items) => result.AddRange(AutoMapper.Mapper.Map<List<QuoteChartingUpdateData>>(items))).ContinueWith(t => { }, ct);

            return result;
        }
    }
}
