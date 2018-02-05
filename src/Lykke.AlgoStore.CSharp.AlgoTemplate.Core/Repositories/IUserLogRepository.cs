using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories
{
    public interface IUserLogRepository
    {
        Task WriteAsync(UserLog userLog);
    }
}
