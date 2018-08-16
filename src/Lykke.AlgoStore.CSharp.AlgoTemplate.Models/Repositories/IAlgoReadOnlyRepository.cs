using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoReadOnlyRepository
    {
        Task<IEnumerable<IAlgo>> GetAllAlgosAsync();
        Task<IEnumerable<IAlgo>> GetAllClientAlgosAsync(string clientId);
        Task<IAlgo> GetAlgoAsync(string clientId, string algoId);
        Task<bool> ExistsAlgoAsync(string clientId, string algoId);
        Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId);
    }
}
