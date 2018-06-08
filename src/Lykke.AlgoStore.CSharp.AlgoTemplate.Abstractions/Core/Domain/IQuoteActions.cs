namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface IQuoteActions : IActions
    {
        double Buy(IAlgoQuote quoteData, double volume);
        double Sell(IAlgoQuote quoteData, double volume);
    }
}
