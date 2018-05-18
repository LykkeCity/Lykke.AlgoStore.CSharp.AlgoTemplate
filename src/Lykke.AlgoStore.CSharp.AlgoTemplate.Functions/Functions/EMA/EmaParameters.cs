using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Functions.EMA
{
    public class EmaParameters : FunctionParamsBase
    {
        /// <summary>
        /// Long term period (window)
        /// </summary>
        public int EmaPeriod { get; set; }
    }
}
