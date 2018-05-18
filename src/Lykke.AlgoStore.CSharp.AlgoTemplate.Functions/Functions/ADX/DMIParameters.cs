using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Functions.ADX
{
    public class DMIParameters : FunctionParamsBase
    {
        public int Period { get; set; }
        public bool IsAverageTrueRangeSet { get; set; }
    }
}
