using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// <see cref="IAlgoQuote"/> implementation
    /// </summary>
    public class AlgoQuote : IAlgoQuote
    {
        public bool IsBuy { get; set; }

        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsOnline { get; set; }
    }
}
