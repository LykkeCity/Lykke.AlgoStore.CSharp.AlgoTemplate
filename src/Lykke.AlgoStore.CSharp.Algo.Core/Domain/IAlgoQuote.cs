using System;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Represents a quote
    /// </summary>
    public interface IAlgoQuote
    {
        /// <summary>
        /// Whether the quote is for a buy or for a sell.
        /// </summary>
        bool IsBuy { get; }

        /// <summary>
        /// The price of the quote
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Time stamp for the quote
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Whether the operation is being processed in real time or not.
        /// If true the quote is latest being processed due to market change
        /// If false the quote is being processed due to algo restore and is 
        /// not the latest quote available
        /// </summary>
        bool IsOnline { get; }
    }
}
