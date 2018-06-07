using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoInstanceTradeRepository
    {
        Task<AlgoInstanceTrade> GetAlgoInstanceOrderAsync(string orderId, string walletId);
        Task CreateAlgoInstanceOrderAsync(AlgoInstanceTrade product);
        Task<IEnumerable<AlgoInstanceTrade>> GetAlgoInstaceTradesByTradedAssetAsync(string instanceId, string assetId, int maxNumberOfRowsToFetch = 0);
        Task SaveAlgoInstanceTradeAsync(AlgoInstanceTrade data);
    }
}
