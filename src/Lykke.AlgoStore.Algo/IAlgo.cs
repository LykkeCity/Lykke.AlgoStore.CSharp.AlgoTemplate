using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo
{
    /// <summary>
    /// A user defined algo.
    /// </summary>
    public interface IAlgo
    {
        /// <summary>
        /// Perform actions on algo startup
        /// </summary>
        void OnStartUp();

        /// <summary>
        /// Perform action on each quote
        /// </summary>
        /// <param name="context">The <see cref="IQuoteContext"/> provided to the algo</param>
        void OnQuoteReceived(IQuoteContext context);

        /// <summary>
        /// Perform action on each candle
        /// </summary>
        /// <param name="context">The <see cref="ICandleContext"/> provided to the algo</param>
        void OnCandleReceived(ICandleContext context);

        /// <summary>
        /// Algo Asset Pair, which will be used for getting candles or quotes information
        /// </summary>
        string AssetPair { get; }
        
        /// <summary>
        /// Time Interval for getting new candles
        /// </summary>
        CandleTimeInterval CandleInterval { get; }

        /// <summary>
        /// The Start Date for which we will get candles or quotes
        /// </summary>
        DateTime StartFrom { get; }

        /// <summary>
        /// The end time on which the algo instance will be shut down
        /// </summary>
        DateTime EndOn { get; }

        /// <summary>
        /// Volume which we will buy or sell
        /// </summary>
        double Volume { get; }

        /// <summary>
        /// Traded asset for the trading service
        /// </summary>
        string TradedAsset { get; }
    }
}
