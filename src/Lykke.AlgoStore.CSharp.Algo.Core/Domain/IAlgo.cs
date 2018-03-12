using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// A user defined algo.
    /// </summary>
    public interface IAlgo
    {
        /// <summary>
        /// Perform actions on algo startup
        /// </summary>
        /// <param name="functions">The algo function provider</param>
        void OnStartUp(IFunctionProvider functions);

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
        string AssetPair { get; set; }
        
        /// <summary>
        /// Time Interval for getting new candles
        /// </summary>
        CandleTimeInterval CandleInterval { get; set; }

        /// <summary>
        /// The Start Date for which we will get candles or quotes
        /// </summary>
        DateTime StartFrom { get; set; }

        /// <summary>
        /// Volume which we will buy or sell
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        /// Traded asset for the trading service
        /// </summary>
        string TradedAsset { get; set; }
    }
}
