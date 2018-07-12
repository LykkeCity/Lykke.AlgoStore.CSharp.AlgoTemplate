using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public abstract class BaseIndicator : IIndicator
    {
        public DateTime StartingDate { get; }
        public DateTime EndingDate { get; }
        public CandleTimeInterval CandleTimeInterval { get; }
        public string AssetPair { get; }

        public abstract double? Value { get; }
        public abstract bool IsReady { get; }

        public BaseIndicator(
            DateTime startingDate, 
            DateTime endingDate, 
            CandleTimeInterval candleTimeInterval,
            string assetPair)
        {
            StartingDate = startingDate;
            EndingDate = endingDate;
            CandleTimeInterval = candleTimeInterval;
            AssetPair = assetPair;
        }

        public abstract double? WarmUp(IEnumerable<Candle> values);
        public abstract double? AddNewValue(Candle value);
    }
}
