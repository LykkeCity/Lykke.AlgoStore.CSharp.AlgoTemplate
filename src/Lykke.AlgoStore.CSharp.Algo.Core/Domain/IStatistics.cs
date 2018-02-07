namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Algo statistics
    /// </summary>
    public interface IStatistics
    {
        double GetBoughtAmount();
        double GetSoldAmount();
        double GetBoughtQuantity();
        double GetSoldQuantity();
    }
}
