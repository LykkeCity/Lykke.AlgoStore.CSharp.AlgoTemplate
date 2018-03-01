using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories
{
    /// <summary>
    /// <see cref="IUserLogRepository"/> implementation
    /// </summary>
    public class UserLogRepository : IUserLogRepository
    {
        private readonly INoSQLTableStorage<UserLogEntity> _table;

        public static readonly string TableName = "CSharpAlgoTemplateUserLog";

        public UserLogRepository(INoSQLTableStorage<UserLogEntity> userLogTableStorage)
        {
            _table = userLogTableStorage;
        }

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey() => String.Format("{0:D19}_{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid());

        public async Task WriteAsync(UserLog userLog)
        {
            var entity = AutoMapper.Mapper.Map<UserLogEntity>(userLog);

            entity.PartitionKey = GeneratePartitionKey(userLog.InstanceId);
            entity.RowKey = GenerateRowKey();

            await _table.InsertOrMergeAsync(entity);
        }

        public async Task<List<UserLog>> GetEntries(int limit, string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(new List<string> { partitionKey }, limit);

            var result = AutoMapper.Mapper.Map<List<UserLog>>(data);

            return result;
        }
    }
}
