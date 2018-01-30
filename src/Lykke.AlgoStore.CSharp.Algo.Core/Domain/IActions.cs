﻿namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IActions
    {
        double BuyStraight(double volume);
        double BuyReverse(double volume);

        double SellStraight(double volume);
        double SellReverse(double volume);

        void Log(string message);
    }
}
