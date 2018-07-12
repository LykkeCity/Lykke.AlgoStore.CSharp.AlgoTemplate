using System;

namespace Lykke.AlgoStore.Algo
{
    public interface ITradeRequest
    {
        double Price { get; set; }
        double Volume { get; set; }
        DateTime Date { get; set; }
    }
}
