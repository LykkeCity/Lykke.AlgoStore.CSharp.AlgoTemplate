namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
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
