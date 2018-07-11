using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo.Indicators
{
    /// <summary>
    /// Interface for a function in the system. The function is producing
    /// result based on both historic data and live data. The function is
    /// is provided with a history data during so called warm-up. And can
    /// be fed with live data.
    /// </summary>
    public interface IIndicator
    {
        /// <summary>
        /// The asset pair on which the function is operating
        /// </summary>
        string AssetPair { get; set; }

        /// <summary>
        /// The start date from which the function should be start to do
        /// calculations.
        /// </summary>
        DateTime StartingDate { get; set; }

        /// <summary>
        /// The end date after which the function will stop receiving updates
        /// </summary>
        DateTime EndingDate { get; set; }

        /// <summary>
        /// The interval of the candles the function is going to work on
        /// Example: If you set it to <see cref="CandleTimeInterval.Minute"/> 
        /// the function will work with candles per minute.
        /// </summary>
        CandleTimeInterval CandleTimeInterval { get; set; }

        double? Value { get; }

        /// <summary>
        /// Whether or not this function has enough data in order to be used properly
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Initialize the function with the initial values.
        /// </summary>
        /// <param name="values">The initial values to be 
        /// computed by the function</param>
        double? WarmUp(IEnumerable<Candle> values);

        /// <summary>
        /// Re-calculates the function with the new value. The value is
        /// presented as <see cref="Candle"/>
        /// </summary>
        /// <param name="value"></param>
        double? AddNewValue(Candle value);
    }
}
