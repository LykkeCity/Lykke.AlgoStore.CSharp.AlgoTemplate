using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain.Contracts;
using OrderAction = Lykke.AlgoStore.Algo.OrderAction;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing trading capabilities
    /// </summary>
    public interface ITradingService
    {
        Task Initialize();
        Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest);
        Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest);
        Task<ResponseModel<LimitOrderResponseModel>> PlaceLimitOrderAsync(ITradeRequest tradeRequest, OrderAction orderAction);
        Task<ResponseModel> CancelLimiOrderAsync(Guid limitOrderId);
    }
}
