namespace Lykke.AlgoStore.Algo
{
    public interface ICandleContext : IContext
    {
        /// <summary>
        /// Access to the data needed by an algo to read incoming candles
        /// <see cref="ICandleData"/>
        /// </summary>
        ICandleData Data { get; }
    }
}
