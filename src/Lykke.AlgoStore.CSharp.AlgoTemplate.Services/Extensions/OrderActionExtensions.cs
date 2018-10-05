using OrderAction = Lykke.AlgoStore.Algo.OrderAction;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class OrderActionExtensions
    {
        public static MatchingEngineAdapter.Abstractions.Domain.OrderAction ToMeaOrderAction(this OrderAction orderAction)
        {
            switch (orderAction)
            {
                case OrderAction.Buy:
                    return MatchingEngineAdapter.Abstractions.Domain.OrderAction.Buy;
                case OrderAction.Sell:
                    return MatchingEngineAdapter.Abstractions.Domain.OrderAction.Sell;
                default:
                    return MatchingEngineAdapter.Abstractions.Domain.OrderAction.Buy;
            }               
        }
    }
}
