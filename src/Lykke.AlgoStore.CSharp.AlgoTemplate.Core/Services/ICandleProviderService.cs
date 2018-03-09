using Autofac;
using Common;
using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Provides a candle feed for given subscriptions
    /// </summary>
    public interface ICandleProviderService : IStopable, IStartable
    {
        /// <summary>
        /// Initializes the <see cref="ICandleProviderService"/>
        /// </summary>
        /// <returns><see cref="Task"/> which will be completed when initialization is complete</returns>
        Task Initialize();

        /// <summary>
        /// Subscribes to candles for a given asset pair and time interval
        /// </summary>
        /// <param name="assetPair">The asset pair to subscribe to</param>
        /// <param name="timeInterval">The time interval of each candle</param>
        /// <param name="callback">The callback which will receive new candles</param>
        void Subscribe(string assetPair, CandleTimeInterval timeInterval, Action<Candle> callback);

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
