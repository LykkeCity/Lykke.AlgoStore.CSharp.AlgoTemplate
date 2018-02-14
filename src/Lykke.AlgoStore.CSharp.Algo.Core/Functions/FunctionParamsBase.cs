using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Functions
{
    /// <summary>
    /// Base class for parameters passed to a function. 
    /// <see cref="IFunction"/>
    /// </summary>
    public class FunctionParamsBase
    {
        /// <summary>
        /// The candle value enumeration. Such as Min/Max Open/Close
        /// </summary>
        public enum CandleValue
        {
            OPEN, CLOSE, LOW, HIGH
        }

        /// <summary>
        /// The asset pair on which the function is operating
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The candle value on which the function is operating. The 
        /// same function can be operating on Min/Max or Open/Close
        /// of a Candle
        /// </summary>
        public CandleValue CandleOperationMode { get; set; }

        /// <summary>
        /// The unique identifier of the running instance for function.
        /// A single function could be instantiated multiple times with
        /// a different parameters during for a single algo. This is the
        /// unique identifier for accessing the function/function results
        /// </summary>
        public string FunctionInstanceIdentifier { get; set; }

        /// <summary>
        /// The start date from which the function should be start to do
        /// calculations.
        /// </summary>
        public DateTime StartingDate { get; set; }

        /// <summary>
        /// The interval of the candles the function is going to work on
        /// Example: If you set it to <see cref="CandleTimeInterval.Minute"/> 
        /// the function will work with candles per minute.
        /// </summary>
        public CandleTimeInterval CandleTimeInterval { get; set; }
    }
}
