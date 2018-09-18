using Lykke.AlgoStore.Algo;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFakeLimitOrdersHandler
    {
        Task HandleLimitOrders(Candle candle);
        Task HandleLimitOrders(IAlgoQuote quotePrice);
        Task Initialize();
    }
}
