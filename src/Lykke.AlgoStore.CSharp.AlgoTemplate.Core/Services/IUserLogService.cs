using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IUserLogService
    {
        void Enqueue(UserLogRequest userLog);
    }
}
