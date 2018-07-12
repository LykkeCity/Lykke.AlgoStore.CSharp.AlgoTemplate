namespace Lykke.AlgoStore.Algo
{
    public interface ICandleActions : IActions
    {
        TradeResponse Buy(IAlgoCandle candleData, double volume);
        TradeResponse Sell(IAlgoCandle candleData, double volume);
    }
}
