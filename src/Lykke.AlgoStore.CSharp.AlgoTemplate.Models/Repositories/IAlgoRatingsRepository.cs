using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoRatingsRepository
    {
        Task<IList<AlgoRatingData>> GetAlgoRatingsAsync(string algoId);
        Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId);
        Task<IList<AlgoRatingData>> GetAlgoRatingsByClientIdAsync(string clientId);
        Task SaveAlgoRatingAsync(AlgoRatingData data);
        Task SaveAlgoRatingWithFakeIdAsync(AlgoRatingData data);
        Task DeleteRatingsAsync(string algoId);
    }
}
