using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IStatisticsRepository
    {
        Task CreateAsync(Statistics data);
        Task DeleteAsync(string instanceId, string id);
        Task<double> GetBoughtAmountAsync(string instanceId);
        Task<double> GetSoldAmountAsync(string instanceId);
        Task<double> GetBoughtQuantityAsync(string instanceId);
        Task<double> GetSoldQuantityAsync(string instanceId);
        Task<int> GetNumberOfRunnings(string instanceId);
    }
}
