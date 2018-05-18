using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Functions.SMA
{
    /// <summary>
    /// Simple Moving Average (SMA) parameters
    /// </summary>
    public class SmaParameters : FunctionParamsBase
    {
        /// <summary>
        /// Short term period (window)
        /// </summary>
        public int ShortTermPeriod { get; set; }

        /// <summary>
        /// Long term period (window)
        /// </summary>
        public int LongTermPeriod { get; set; }

        /// <summary>
        /// Number of decimals
        /// </summary>
        public int? Decimals { get; set; }

        /// <summary>
        /// Function capacity (max number of values that will be used for calculus)
        /// </summary>
        public int Capacity { get; set; }
    }
}
