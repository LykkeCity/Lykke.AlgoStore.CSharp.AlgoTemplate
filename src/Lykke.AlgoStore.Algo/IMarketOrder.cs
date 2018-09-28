using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo
{
    public interface IMarketOrder
    {
        OrderAction Action { get; }
        OrderStatus Status { get; }
        double Volume { get; }

        event Action<IMarketOrder, IContext> OnFulfilled;
        event Action<IMarketOrder, TradeErrorCode, string, IContext> OnErrored;
    }
}
