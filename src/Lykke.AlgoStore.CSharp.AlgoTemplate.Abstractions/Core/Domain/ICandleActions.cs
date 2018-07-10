using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public interface ICandleActions : IActions
    {
        TradeResponse Buy(IAlgoCandle candleData, double volume);
        TradeResponse Sell(IAlgoCandle candleData, double volume);
    }
}
