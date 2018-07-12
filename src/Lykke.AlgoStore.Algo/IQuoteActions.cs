namespace Lykke.AlgoStore.Algo
{
    public interface IQuoteActions : IActions
    {
        TradeResponse Buy(IAlgoQuote quoteData, double volume);
        TradeResponse Sell(IAlgoQuote quoteData, double volume);
    }
}
