using Autofac;
using Common;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Provides a candle feed for given subscriptions
    /// </summary>
    public interface ICandleProviderService
    {
        /// <summary>
        /// Initializes the <see cref="ICandleProviderService"/>
        /// </summary>
        /// <returns><see cref="Task"/> which will be completed when initialization is complete</returns>
        Task Initialize();

        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Subscribes to candles for a given asset pair and time interval
        /// </summary>
        /// <param name="serviceRequest">The candle service request containing data about the subscription</param>
        /// <param name="callback">The callback which will receive new candles</param>
        void Subscribe(CandleServiceRequest serviceRequest, Action<Candle> callback);

        /// <summary>
        /// Sets the previous candle for an asset pair and time interval
        /// </summary>
        /// <remarks>
        /// Used when the candle provider lacks historical information to generate empty candles when 
        /// new candles do not come for a given period
        /// </remarks>
        /// <param name="assetPair">The asset pair to set the previous candle for</param>
        /// <param name="timeInterval">The time interval of the asset pair to set the previous candle for</param>
        /// <param name="candle">The candle to set</param>
        void SetPrevCandleFromHistory(string assetPair, CandleTimeInterval timeInterval, Candle candle);
    }
}
