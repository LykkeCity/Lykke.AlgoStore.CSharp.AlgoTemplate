using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IUserLogService
    {
        //Task Write(UserLog userLog);
        Task Write(string instanceId, Exception exception);
        Task Write(string instanceId, string message);
    }
}
