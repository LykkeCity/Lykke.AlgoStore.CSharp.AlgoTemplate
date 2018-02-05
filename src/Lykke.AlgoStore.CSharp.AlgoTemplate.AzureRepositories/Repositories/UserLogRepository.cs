using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly INoSQLTableStorage<UserLogEntity> _table;

        public static readonly string TableName = "CSharpAlgoTemplateUserLog";

        public UserLogRepository(INoSQLTableStorage<UserLogEntity> userLogTableStorage)
        {
            _table = userLogTableStorage;
        }

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(DateTime date) => date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

        public async Task WriteAsync(UserLog userLog)
        {
            var entity = AutoMapper.Mapper.Map<UserLogEntity>(userLog);

            entity.PartitionKey = GeneratePartitionKey(userLog.AlgoId);
            entity.RowKey = GenerateRowKey(userLog.Date);

            await _table.InsertOrMergeAsync(entity);
        }
    }
}
