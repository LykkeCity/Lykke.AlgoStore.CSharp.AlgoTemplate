namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// A user defined algo.
    /// </summary>
    public interface IAlgo
    {
        /// <summary>
        /// Perform actions on algo startup
        /// </summary>
        /// <param name="functions">The algo function provider</param>
        void OnStartUp(IFunctionProvider functions);

        /// <summary>
        /// Perform action on each quote
        /// </summary>
        /// <param name="context">The <see cref="IQuoteContext"/> provided to the algo</param>
        void OnQuoteReceived(IQuoteContext context);

        /// <summary>
        /// Perform action on each candle
        /// </summary>
        /// <param name="context">The <see cref="ICandleContext"/> provided to the algo</param>
        void OnCandleReceived(ICandleContext context);
    }
}
