using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Functions
{
    /// <summary>
    /// Interface for a function in the system. The function is producing
    /// result based on both historic data and live data. The function is
    /// is provided with a history data during so called warm-up. And can
    /// be fed with live data.
    /// </summary>
    public interface IFunction
    {
        FunctionParamsBase FunctionParameters { get; }

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
