using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.ADX
{
    public class DMIParameters : FunctionParamsBase
    {
        public int Period { get; set; }
        public bool IsAverageTrueRangeSet { get; set; }
    }
}
