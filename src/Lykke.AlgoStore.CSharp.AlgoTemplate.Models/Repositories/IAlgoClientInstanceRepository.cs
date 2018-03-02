using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoClientInstanceRepository
    {
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByAlgoAsync(string algoId);
        Task<List<AlgoClientInstanceData>> GetAllAlgoInstancesByClientAsync(string clientId);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataByAlgoIdAsync(string algoId, string instanceId);
        Task<AlgoClientInstanceData> GetAlgoInstanceDataByClientIdAsync(string clientId, string instanceId);

        Task<bool> ExistsAlgoInstanceDataWithAlgoIdAsync(string algoId, string instanceId);
        Task<bool> ExistsAlgoInstanceDataWithClientIdAsync(string clientId, string instanceId);        
        Task SaveAlgoInstanceDataAsync(AlgoClientInstanceData data);
        Task DeleteAlgoInstanceDataAsync(AlgoClientInstanceData metaData);

        Task<string> GetAlgoInstanceMetadataSetting(string algoId, string instanceId, string key);       

    }
}
