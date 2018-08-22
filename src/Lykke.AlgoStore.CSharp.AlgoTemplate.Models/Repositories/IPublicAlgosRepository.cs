using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IPublicAlgosRepository
    {
        Task<List<PublicAlgoData>> GetAllPublicAlgosAsync();
        Task<bool> ExistsPublicAlgoAsync(string clientId, string algoId);
        Task SavePublicAlgoAsync(PublicAlgoData data);
        Task DeletePublicAlgoAsync(PublicAlgoData data);
        Task SavePublicAlgoNewPKAsync(PublicAlgoData data);
    }
}
