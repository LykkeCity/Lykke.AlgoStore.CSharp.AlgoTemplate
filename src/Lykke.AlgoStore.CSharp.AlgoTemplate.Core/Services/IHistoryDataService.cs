using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// A service for accessing the History data of market prices <see cref="Candle"/>
    /// The service is providing the history data for assets as a well as a ability to
    /// subscribe and receive updates for the history data
    /// </summary>
    public interface IHistoryDataService
    {
        /// <summary>
        /// Get the history data as a sequence of <see cref="Candle"/>
        /// </summary>
        /// <param name="request">The request parameters as a <see cref="CandlesHistoryRequest"/></param>
        /// <param name="historyUpdateSubscriber"></param>
        /// <returns></returns>
        IEnumerable<Candle> GetHistory(CandlesHistoryRequest request);
    }
}
