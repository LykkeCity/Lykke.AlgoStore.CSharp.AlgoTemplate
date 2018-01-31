namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// A user defined algo.
    /// </summary>
    public interface IAlgo
    {
        /// <summary>
        /// Perform action on each quote
        /// </summary>
        /// <param name="context">The <see cref="IContext"/> provided to the algo</param>
        void OnQuoteReceived(IContext context);
    }
}
