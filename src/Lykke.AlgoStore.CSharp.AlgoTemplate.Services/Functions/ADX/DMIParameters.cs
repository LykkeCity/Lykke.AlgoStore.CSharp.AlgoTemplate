using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class DMIParameters : FunctionParamsBase
    {
        public int Priod { get; set; }
        public int Samples { get; set; }
        public bool IsAverageTrueRangeSet { get; set; }
    }
}
