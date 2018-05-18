namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain
{
    public interface ICandleActions : IActions
    {
        double Buy(IAlgoCandle candleData, double volume);
        double Sell(IAlgoCandle candleData, double volume);
    }
}
