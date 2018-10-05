namespace Lykke.AlgoStore.Algo
{
    public interface IOrderProvider
    {
        IMarketOrderManager Market { get; }
        ILimitOrderManager Limit { get; }
    }
}
