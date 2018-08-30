using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Algo
{
    public interface ILimitOrderManager : IReadOnlyList<IEditableOrder>
    {
        IMarketOrder Create(OrderAction action, double volume, double price);
    }
}
