using System;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Represents a quote
    /// </summary>
    public interface IAlgoQuote
    {
        bool IsBuy { get; }
        double Price { get; }
        DateTime Timestamp { get; }
        bool IsOnline { get; }
    }
}
