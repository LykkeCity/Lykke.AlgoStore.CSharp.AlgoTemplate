using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo
{
    public interface IMarketOrderManager : IReadOnlyList<IMarketOrder>, IDisposable
    {
        IMarketOrder Create(OrderAction action, double volume);
    }
}
