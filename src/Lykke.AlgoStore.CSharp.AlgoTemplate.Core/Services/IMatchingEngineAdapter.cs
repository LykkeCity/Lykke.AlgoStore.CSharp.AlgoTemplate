using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using System.Threading.Tasks;
using OrderAction = Lykke.MatchingEngine.Connector.Abstractions.Models.OrderAction;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IMatchingEngineAdapter
    {
        //Task<ResponseModel> CancelLimitOrderAsync(Guid limitOrderId);
        Task<ResponseModel<double>> HandleMarketOrderAsync(string clientId, string assetPairId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = default(double?));
        //Task<ResponseModel<Guid>> PlaceLimitOrderAsync(string clientId, string assetPairId, OrderAction orderAction, double volume, double price, bool cancelPreviousOrders = false);
    }
}
