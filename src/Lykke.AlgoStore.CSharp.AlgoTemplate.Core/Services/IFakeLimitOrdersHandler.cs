using Lykke.AlgoStore.Algo;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IFakeLimitOrdersHandler
    {
        Task HandleLimitOrders(Candle candle, IContext context);
        Task HandleLimitOrders(IAlgoQuote quotePrice, IContext context);
        Task Initialize();
    }
}
