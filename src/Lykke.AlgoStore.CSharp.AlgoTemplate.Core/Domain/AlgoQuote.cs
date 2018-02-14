using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// <see cref="IAlgoQuote"/> implementation
    /// </summary>
    public class AlgoQuote : IAlgoQuote
    {
        public bool IsBuy { get; set; }

        public double Price { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime DateReceived { get; set; }

        public bool IsOnline { get; set; }
    }
}
