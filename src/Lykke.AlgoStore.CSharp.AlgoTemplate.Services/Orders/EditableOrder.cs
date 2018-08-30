using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class EditableOrder : MarketOrder
    {
        public double Price { get; set; }

        protected EditableOrder(OrderAction action, double volume)
        : base(action, volume)
        {

        }
    }
}
