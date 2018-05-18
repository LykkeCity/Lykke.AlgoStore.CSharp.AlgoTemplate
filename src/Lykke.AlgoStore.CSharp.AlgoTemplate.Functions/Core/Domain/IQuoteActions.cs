namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface IQuoteActions : IActions
    {
        double Buy(double volume);
        double Sell(double volume);
    }
}
