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
    public class FunctionChartingUpdateRepository : IFunctionChartingUpdateRepository
    {
        private readonly INoSQLTableStorage<FunctionChartingUpdateEntity> _table;
        private static string GeneratePartitionKey(string key) => key;

        public static readonly string TableName = "AlgoInstanceFunctionsChartingTable";

        public FunctionChartingUpdateRepository(INoSQLTableStorage<FunctionChartingUpdateEntity> table)
        {
            _table = table;
        }

        public async Task<IEnumerable<FunctionChartingUpdateData>> GetFunctionChartingUpdateForPeriodAsync(string instanceId, DateTime from, DateTime to, CancellationToken ct)
        {
            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GeneratePartitionKey(instanceId));
            string fromFilter = TableQuery.GenerateFilterConditionForDate("CalculatedOn", QueryComparisons.GreaterThanOrEqual, from);
            string toFilter = TableQuery.GenerateFilterConditionForDate("CalculatedOn", QueryComparisons.LessThanOrEqual, to);

            var query = new TableQuery<FunctionChartingUpdateEntity>().Where(TableQuery.CombineFilters(TableQuery.CombineFilters(pkFilter, TableOperators.And, fromFilter), TableOperators.And, toFilter));

            var result = new List<FunctionChartingUpdateData>();

            await _table.GetDataByChunksAsync(query, (items) => result.AddRange(AutoMapper.Mapper.Map<List<FunctionChartingUpdateData>>(items))).ContinueWith(t => { }, ct);

            return result;
        }
    }
}
