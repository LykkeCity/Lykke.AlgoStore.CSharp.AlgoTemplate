namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IQuoteActions : IActions
    {
        double Buy(double volume);
        double Sell(double volume);
    }
}
