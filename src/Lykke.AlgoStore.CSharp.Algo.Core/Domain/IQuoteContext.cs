namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IQuoteContext : IContext
    {
        /// <summary>
        /// Access to the data needed by an algo to read incoming quotes
        /// <see cref="IQuoteData"/>
        /// </summary>
        IQuoteData Data { get; }
    }
}
