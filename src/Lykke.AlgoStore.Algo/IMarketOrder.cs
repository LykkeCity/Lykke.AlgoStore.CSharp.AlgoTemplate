﻿using System;

namespace Lykke.AlgoStore.Algo
{
    public interface IMarketOrder
    {
        OrderAction Action { get; }
        OrderStatus Status { get; }
        double Volume { get; }

        event Action<IMarketOrder> OnFulfilled;
        event Action<IMarketOrder, TradeErrorCode, string> OnErrored;

        void Cancel();
    }
}
