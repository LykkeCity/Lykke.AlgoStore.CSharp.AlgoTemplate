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
        double VolumeFulfilled { get; set; }
        double Price { get; }

        event Action<ILimitOrder, IContext> OnFulfilled;
        event Action<ILimitOrder, IContext> OnPartiallyFulfilled;
        event Action<ILimitOrder, IContext> OnRegistered;
        event Action<ILimitOrder, IContext> OnCancelled;
        event Action<ILimitOrder, TradeErrorCode, string, IContext> OnErrored;

        void MarkFulfilled(IContext context);
        void MarkPartiallyFulfilled(double amountFullfilled, IContext context);
        void MarkPlaced(IContext context);
    }
}
