﻿using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// <see cref="ICandleData"/> implementation of the algo candle data
    /// </summary>
    public class AlgoCandleData : ICandleData
    {
        public IAlgoCandle Candle { get; }

        /// <summary>
        /// Initializes new instance of the <see cref="AlgoCandleData"/>
        /// </summary>
        /// <param name="candle">The <see cref="IAlgoCandle"/> implementation</param>
        public AlgoCandleData(IAlgoCandle candle)
        {
            Candle = candle;
        }
    }
}
