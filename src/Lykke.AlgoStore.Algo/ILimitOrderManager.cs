using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo
{
    public interface ILimitOrderManager : IReadOnlyList<ILimitOrder>, IDisposable
    {
        ILimitOrder Create(OrderAction action, double volume, double price, IContext context);
        ILimitOrder Cancel(Guid limitOrderId);
    }
}
