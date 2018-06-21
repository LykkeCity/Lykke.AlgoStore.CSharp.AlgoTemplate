using Common;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public class TradeRequest : ITradeRequest
    {
        public double Price { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return $"Price: {Price}, Volume: {Volume}, Date: {Date.ToIsoDateTime()}";
        }
    }
}
