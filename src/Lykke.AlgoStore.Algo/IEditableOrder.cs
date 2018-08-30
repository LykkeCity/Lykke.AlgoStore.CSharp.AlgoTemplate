using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Algo
{
    public interface IEditableOrder : IMarketOrder
    {
        double Price { get; set; }
    }
}
