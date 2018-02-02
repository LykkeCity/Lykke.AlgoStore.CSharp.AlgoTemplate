namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Algo statistics
    /// </summary>
    public interface IStatistics
    {
        double GetBoughtAmount();
        double GetBoughtAmountByAsset(string assetName);
        double GetSoldAmount();
        double GetSoldAmountByAsset(string assetName);
        double GetBoughtQuantityByAsset(string assetName);
    }
}
