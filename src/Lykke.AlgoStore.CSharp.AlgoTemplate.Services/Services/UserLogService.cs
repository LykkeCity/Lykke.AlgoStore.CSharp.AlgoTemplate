using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IUserLogService"/> implementation
    /// </summary>
    public class UserLogService : IUserLogService
    {
        private readonly IUserLogRepository _userLogRepository;

        public UserLogService(IUserLogRepository userLogRepository)
        {
            _userLogRepository = userLogRepository;
        }

        public async Task Write(UserLog userLog)
        {
            await _userLogRepository.WriteAsync(userLog);
        }
    }
}
