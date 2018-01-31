namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Represents the algo actions, which can be executed.
    /// </summary>
    public interface IActions
    {
        double BuyStraight(double volume);
        double BuyReverse(double volume);

        double SellStraight(double volume);
        double SellReverse(double volume);

        void Log(string message);
    }
}
