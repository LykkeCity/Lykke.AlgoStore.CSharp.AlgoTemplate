using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoInstanceTradeRepository
    {
        Task<AlgoInstanceTrade> GetAlgoInstanceOrderAsync(string orderId, string walletId);
        Task CreateOrUpdateAlgoInstanceOrderAsync(AlgoInstanceTrade product);
        Task<IEnumerable<AlgoInstanceTrade>> GetAlgoInstaceTradesByTradedAssetAsync(string instanceId, string assetId, int maxNumberOfRowsToFetch = 0);
        Task SaveAlgoInstanceTradeAsync(AlgoInstanceTrade data);

        Task<IEnumerable<AlgoInstanceTrade>> GetInstaceTradesByTradedAssetAndPeriodAsync(string instanceId, string assetId, DateTime from, DateTime to, CancellationToken ct);
    }
}
