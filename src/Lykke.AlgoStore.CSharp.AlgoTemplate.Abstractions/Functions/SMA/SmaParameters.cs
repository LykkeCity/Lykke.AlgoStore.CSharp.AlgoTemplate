using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA
{
    /// <summary>
    /// Simple Moving Average (SMA) parameters
    /// </summary>
    public class SmaParameters : FunctionParamsBase
    {
        /// <summary>
        /// Function capacity (max number of values that will be used for calculus)
        /// </summary>
        public int Capacity { get; set; }
    }
}
