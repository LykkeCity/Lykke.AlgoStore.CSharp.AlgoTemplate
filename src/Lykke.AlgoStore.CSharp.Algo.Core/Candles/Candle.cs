using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Candles
{
    public class Candle : IAlgoCandle
    {
        public DateTime DateTime { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double TradingVolume { get; set; }

        public double TradingOppositeVolume { get; set; }

        public double LastTradePrice { get; set; }
    }
}
