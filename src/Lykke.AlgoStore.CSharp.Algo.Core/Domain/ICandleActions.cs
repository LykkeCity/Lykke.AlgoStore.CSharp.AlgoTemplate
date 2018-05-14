﻿namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface ICandleActions : IActions
    {
        double Buy(IAlgoCandle candleData, double volume);
        double Sell(IAlgoCandle candleData, double volume);
    }
}
