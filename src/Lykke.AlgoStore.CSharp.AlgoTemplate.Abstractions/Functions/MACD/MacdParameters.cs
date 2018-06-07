using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.MACD
{
    public class MacdParameters : FunctionParamsBase
    {
        /// <summary>
        /// The slow moving EMA period
        /// </summary>
        public int SlowEmaPeriod { get; set; }

        /// <summary>
        /// The fast moving EMA period
        /// </summary>
        public int FastEmaPeriod { get; set; }

        /// <summary>
        /// The signal line EMA period
        /// </summary>
        public int SignalLinePeriod { get; set; }
    }
}
