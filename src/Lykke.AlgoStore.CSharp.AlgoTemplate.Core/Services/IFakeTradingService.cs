using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFakeTradingService
    {
        Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest);
        Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest);
        Task Initialize(string instanceId, string assetPairId, bool straight);

        decimal TradedAssetBalance { get; }
        decimal OppositeAssetBalance { get; }
    }
}
