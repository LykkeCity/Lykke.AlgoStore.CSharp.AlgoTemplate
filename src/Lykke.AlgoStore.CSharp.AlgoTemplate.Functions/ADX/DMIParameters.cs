using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.ADX
{
    public class DMIParameters : FunctionParamsBase
    {
        public int Period { get; set; }
        public bool IsAverageTrueRangeSet { get; set; }
    }
}
