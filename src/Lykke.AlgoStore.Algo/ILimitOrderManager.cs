using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo
{
    public interface ILimitOrderManager : IReadOnlyList<ILimitOrder>
    {
        ILimitOrder Create(OrderAction action, double volume, double price);
        ILimitOrder Cancel(Guid limitOrderId);
    }
}
