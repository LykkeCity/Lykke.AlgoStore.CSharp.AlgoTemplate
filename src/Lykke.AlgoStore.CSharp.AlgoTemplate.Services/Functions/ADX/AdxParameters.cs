using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class AdxParameters : FunctionParamsBase
    {
        public int AdxPriod { get; set; }

        public int Samples { get; set; }

        public double DirectionalMovementPlus { get; set; }
        public double DirectionalMovementMinus { get; set; }
    }
}
