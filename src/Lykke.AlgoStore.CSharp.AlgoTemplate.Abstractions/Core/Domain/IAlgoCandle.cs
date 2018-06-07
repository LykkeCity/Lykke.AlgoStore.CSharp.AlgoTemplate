using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface IAlgoCandle
    {
        DateTime DateTime { get; }

        double Open { get; }

        double Close { get; }

        double High { get; }

        double Low { get; }

        double TradingVolume { get; }

        double TradingOppositeVolume { get; }

        double LastTradePrice { get; }
    }
}
