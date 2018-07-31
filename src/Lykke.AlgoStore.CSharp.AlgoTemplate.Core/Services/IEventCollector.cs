using Lykke.AlgoStore.Algo.Charting;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IEventCollector
    {
        Task SubmitTradeEvent(TradeChartingUpdate trade);
        Task SubmitCandleEvent(CandleChartingUpdate candle);
        Task SubmitFunctionEvent(FunctionChartingUpdate function);
    }
}
