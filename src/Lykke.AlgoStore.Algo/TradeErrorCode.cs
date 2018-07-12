namespace Lykke.AlgoStore.Algo
{
    public enum TradeErrorCode
    {
        LowBalance = 401,
        AlreadyProcessed = 402,
        UnknownAsset = 410,
        NoLiquidity = 411,
        NotEnoughFunds = 412,
        ReservedVolumeHigherThanBalance = 414,
        BalanceLowerThanReserved = 416,
        LeadToNegativeSpread = 417,
        Runtime = 500,
        NetworkError = 501,
        RequestTimeout = 502
    }
}
