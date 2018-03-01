using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories
{
    public interface IUserLogRepository
    {
        Task WriteAsync(UserLog userLog);
        Task WriteAsync(string instanceId, string message);
        Task WriteAsync(string instanceId, Exception exception);
        Task<List<UserLog>> GetEntries(int limit, string instanceId);
    }
}
