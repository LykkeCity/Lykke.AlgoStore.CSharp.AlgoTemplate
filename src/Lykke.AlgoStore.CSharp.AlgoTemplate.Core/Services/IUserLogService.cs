using Lykke.Service.Logging.Client.AutorestClient.Models;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IUserLogService : IDisposable
    {
        void Enqueue(UserLogRequest userLog);
        void Enqueue(string instanceId, string message);
    }
}
