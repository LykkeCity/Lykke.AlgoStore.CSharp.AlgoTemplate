using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFakeTradingService
    {
        Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData);
        Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData);
        void Initialize(string instanceId, string assetPairId, bool straight);

    }
}
