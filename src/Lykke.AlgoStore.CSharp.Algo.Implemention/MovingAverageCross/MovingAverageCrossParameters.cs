using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention.MovingAverageCross
{
    public class MovingAverageCrossParameters : FunctionParamsBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ADXThreshold { get; set; }
    }
}
