using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    /// <summary>
    /// <see cref="IUserLogRepository"/> implementation
    /// </summary>
    public class UserLogRepository : IUserLogRepository
    {
        private readonly INoSQLTableStorage<UserLogEntity> _table;

        private readonly object _sync = new object();
        private long _lastDifference = -1;
        private int _duplicateCounter = 99999;

        public static readonly string TableName = "CSharpAlgoTemplateUserLog";

        public UserLogRepository(INoSQLTableStorage<UserLogEntity> userLogTableStorage)
        {
            _table = userLogTableStorage;
        }

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(long difference, int duplicateCounter) => String.Format("{0:D19}{1:D5}_{2}", difference, duplicateCounter, Guid.NewGuid());

        public async Task WriteAsync(UserLog userLog)
        {
            var entity = AutoMapper.Mapper.Map<UserLogEntity>(userLog);

            entity.PartitionKey = GeneratePartitionKey(userLog.InstanceId);
            entity.RowKey = GenerateRowKey();

            await _table.InsertOrMergeAsync(entity);
        }

        public async Task WriteAsync(string instanceId, string message)
        {
            var entity = new UserLogEntity
            {
                Message = message,
                Date = DateTime.UtcNow,

                PartitionKey = GeneratePartitionKey(instanceId),
                RowKey = GenerateRowKey()
            };

            await _table.InsertOrMergeAsync(entity);
        }

        public async Task WriteAsync(string instanceId, Exception exception)
        {
            await WriteAsync(instanceId, exception.ToString());
        }

        public async Task<List<UserLog>> GetEntries(int limit, string instanceId)
        {
            var partitionKey = GeneratePartitionKey(instanceId);
            var data = await _table.GetDataAsync(new List<string> { partitionKey }, limit);

            var result = AutoMapper.Mapper.Map<List<UserLog>>(data);

            return result;
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
    }
}
