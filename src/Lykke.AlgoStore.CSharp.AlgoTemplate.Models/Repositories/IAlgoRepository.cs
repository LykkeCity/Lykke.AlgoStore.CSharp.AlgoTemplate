using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoRepository : IAlgoReadOnlyRepository
    {
        Task SaveAlgoAsync(IAlgo metaData);

        Task SaveAlgoWithNewPKAsync(IAlgo algo, string oldPK);

        Task DeleteAlgoAsync(string clientId, string algoId);
    }
}
