using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing capabilities to retrieve and monitor Candles
    /// </summary>
    public interface ICandlesService
    {
        /// <summary>
        /// Subscribe for Candle values. You can provide a list of  <see cref="CandleServiceRequest"/>
        /// and a action for the initial candle data as well as an action to be notified when a new
        /// candle is available
        /// </summary>
        /// <param name="candleServiceRequests">The <see cref="CandleServiceRequest"/></param>
        /// <param name="initalDataConsumer">An action to receive the initial candle data for the requests</param>
        /// <param name="candleUpdateConsumer">An action to be invoked when a new candle is available</param>
        void Subscribe(IList<CandleServiceRequest> candleServiceRequests,
            Action<IList<MultipleCandlesResponse>> initalDataConsumer,
            Action<IList<SingleCandleResponse>> candleUpdateConsumer);

        void StartProducing();

        void StopProducing();
    }
}
