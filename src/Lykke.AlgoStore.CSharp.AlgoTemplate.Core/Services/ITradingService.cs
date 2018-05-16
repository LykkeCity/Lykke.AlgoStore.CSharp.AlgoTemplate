using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing trading capabilities
    /// </summary>
    public interface ITradingService
    {
        void Initialize();
        Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData = null);
        Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData = null);
    }
}
