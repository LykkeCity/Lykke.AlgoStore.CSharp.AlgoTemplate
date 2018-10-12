using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    public class OrderProvider : IOrderProvider
    {
        public IMarketOrderManager Market { get; }
        public ILimitOrderManager Limit { get; }

        public OrderProvider(
            IMarketOrderManager marketOrderManager,
            ILimitOrderManager limitOrderManager)
        {
            Market = marketOrderManager;
            Limit = limitOrderManager;
        }
    }
}
