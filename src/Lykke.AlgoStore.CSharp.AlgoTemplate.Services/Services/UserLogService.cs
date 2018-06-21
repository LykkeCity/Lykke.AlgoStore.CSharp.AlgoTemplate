using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IUserLogService"/> implementation
    /// </summary>
    public class UserLogService : IUserLogService
    {
        private readonly ILoggingClient _userLogRepository;

        public UserLogService(ILoggingClient userLogRepository)
        {
            _userLogRepository = userLogRepository;
        }

        //public async Task Write(UserLog userLog)
        //{
        //    await _userLogRepository.WriteAsync(userLog);
        //}

        public async Task Write(string instanceId, Exception exception)
        {
            await _userLogRepository.WriteAsync(instanceId, exception.ToString());
        }

        public async Task Write(string instanceId, string message)
        {
            await _userLogRepository.WriteAsync(instanceId, message);
        }
    }
}
