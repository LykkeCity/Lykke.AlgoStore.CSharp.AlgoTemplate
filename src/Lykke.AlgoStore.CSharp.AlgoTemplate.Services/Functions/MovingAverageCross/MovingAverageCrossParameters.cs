using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MovingAverageCross
{
    public class MovingAverageCrossParameters : FunctionParamsBase
    {
        /// <summary>
        /// SMA Short term period
        /// </summary>
        public int ShortTermPeriod { get; set; }

        /// <summary>
        /// SMA Long term period 
        /// </summary>
        public int LongTermPeriod { get; set; }

        /// <summary>
        /// ADX Peri 
        /// </summary>
        public int AdxPeriod { get; set; }

        /// <summary>
        /// Function capacity (max number of values that will be used for calculus)
        /// </summary>
        public int Capacity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ADXThreshold { get; set; }
    }
}
