namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface ITradingService
    {
        void Initialise();

        double BuyStraight(double volume);
        double BuyReverse(double volume);

        double SellStraight(double volume);
        double SellReverse(double volume);
    }
}
