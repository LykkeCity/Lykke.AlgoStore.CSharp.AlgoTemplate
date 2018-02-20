﻿using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.EMA
{
    public class EmaParameters : FunctionParamsBase
    {
        /// <summary>
        /// Long term period (window)
        /// </summary>
        public int EmaPeriod { get; set; }
    }
}
