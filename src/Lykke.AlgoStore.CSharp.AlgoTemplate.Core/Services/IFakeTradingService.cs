using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain.Contracts;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFakeTradingService
    {
        Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest);
        Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest);
        Task<ResponseModel<LimitOrderResponseModel>> PlaceLimitOrderAsync(ITradeRequest tradeRequest, bool isBuy);
        Task Initialize(string instanceId, string assetPairId, bool straight);
    }
}
