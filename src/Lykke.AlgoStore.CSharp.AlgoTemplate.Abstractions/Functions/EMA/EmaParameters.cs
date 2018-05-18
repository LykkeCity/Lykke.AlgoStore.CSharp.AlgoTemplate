using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.EMA
{
    public class EmaParameters : FunctionParamsBase
    {
        /// <summary>
        /// Long term period (window)
        /// </summary>
        public int EmaPeriod { get; set; }
    }
}
