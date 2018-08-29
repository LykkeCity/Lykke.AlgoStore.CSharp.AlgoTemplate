using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo
{
    public interface IMarketOrderManager : IReadOnlyList<IMarketOrder>
    {
        IMarketOrder Create(OrderAction action, double volume);
    }
}
