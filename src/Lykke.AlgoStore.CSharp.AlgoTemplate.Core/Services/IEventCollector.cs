using Lykke.AlgoStore.Algo.Charting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IEventCollector
    {
        /// <summary>
        /// Submits a trade event to the event handler
        /// </summary>
        /// <param name="trade">The trade event to submit</param>
        /// <returns>Task which completes once the event has been submitted</returns>
        Task SubmitTradeEvent(TradeChartingUpdate trade);
        /// <summary>
        /// Submits a set of trade events to the event handler
        /// </summary>
        /// <param name="trades">The set of trade events to submit</param>
        /// <returns>Task which completes once the events have been submitted</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="trades"/> is null
        /// </exception>
        Task SubmitTradeEvents(IEnumerable<TradeChartingUpdate> trades);

        /// <summary>
        /// Submits a candle event to the event handler
        /// </summary>
        /// <param name="candle">The candle event to submit</param>
        /// <returns>Task which completes once the event has been submitted</returns>
        Task SubmitCandleEvent(CandleChartingUpdate candle);
        /// <summary>
        /// Submits a set of candle events to the event handler
        /// </summary>
        /// <param name="candles">The set of candle events to submit</param>
        /// <returns>Task which completes once the events have been submitted</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="candles"/> is null
        /// </exception>
        Task SubmitCandleEvents(IEnumerable<CandleChartingUpdate> candles);

        /// <summary>
        /// Submits a function update event to the event handler
        /// </summary>
        /// <param name="function">The function update event to submit</param>
        /// <returns>Task which completes once the event has been submitted</returns>
        Task SubmitFunctionEvent(FunctionChartingUpdate function);
        /// <summary>
        /// Submits a set of function update events to the event handler
        /// </summary>
        /// <param name="functions">The set of function update events to submit</param>
        /// <returns>Task which completes once the events have been submitted</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="functions"/> is null
        /// </exception>
        Task SubmitFunctionEvents(IEnumerable<FunctionChartingUpdate> functions);
    }
}
