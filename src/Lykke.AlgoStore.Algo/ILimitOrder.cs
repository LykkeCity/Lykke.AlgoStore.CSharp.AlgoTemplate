using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo
{
    public interface ILimitOrder
    {
        OrderAction Action { get; }
        OrderStatus Status { get; }
        Guid Id { get; }
        double Volume { get; }
        double Price { get; }

        event Action<ILimitOrder> OnFulfilled;
        event Action<ILimitOrder> OnRegistered;
        event Action<ILimitOrder> OnCancelled;
        event Action<ILimitOrder, TradeErrorCode, string> OnErrored;

        void Cancel();
    }
}
