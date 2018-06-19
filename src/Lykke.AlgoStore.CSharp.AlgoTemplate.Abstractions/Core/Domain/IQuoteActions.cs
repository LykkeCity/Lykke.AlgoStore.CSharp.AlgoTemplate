using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface IQuoteActions : IActions
    {
        TradeResponse Buy(IAlgoQuote quoteData, double volume);
        TradeResponse Sell(IAlgoQuote quoteData, double volume);
    }
}
