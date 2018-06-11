using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing trading capabilities
    /// </summary>
    public interface ITradingService
    {
        void Initialize();
        Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest);
        Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest);
    }
}
