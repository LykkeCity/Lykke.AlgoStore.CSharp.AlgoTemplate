using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories
{
    public interface IStatisticsRepository
    {
        Task CreateAsync(Statistics data);
        Task DeleteAsync(string instanceId, string id);
    }
}
