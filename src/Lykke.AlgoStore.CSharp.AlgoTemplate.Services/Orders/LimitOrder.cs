using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class LimitOrder : EditableOrder
    {
        public LimitOrder(OrderAction action, double volum, double price)
        : base(action, volum)
        {
            Price = price;
        }
    }
}
