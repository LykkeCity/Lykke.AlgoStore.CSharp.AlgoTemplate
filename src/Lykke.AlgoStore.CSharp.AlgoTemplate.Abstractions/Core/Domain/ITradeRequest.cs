using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface ITradeRequest
    {
        double Price { get; set; }
        double Volume { get; set; }
        DateTime Date { get; set; }
    }
}
