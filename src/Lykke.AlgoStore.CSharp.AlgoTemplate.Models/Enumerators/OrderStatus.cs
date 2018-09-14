namespace Lykke.AlgoStore.Algo
{
    public enum OrderStatus
    {
        UnknownStatus = 0,
        Placed = 1,
        PartiallyMatched = 2,
        Matched = 3,
        Pending = 4,
        Cancelled = 5,
        Replaced = 6,
        Rejected = 7,
        Errored = 8
    }
}
