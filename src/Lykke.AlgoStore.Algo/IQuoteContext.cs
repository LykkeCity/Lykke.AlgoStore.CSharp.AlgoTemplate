namespace Lykke.AlgoStore.Algo
{
    public interface IQuoteContext : IContext
    {
        /// <summary>
        /// Access to the data needed by an algo to read incoming quotes
        /// <see cref="IQuoteData"/>
        /// </summary>
        IQuoteData Data { get; }

        /// <summary>
        /// Access point for the user action, which can be performed 
        /// by an algo <see cref="IActions"/>
        /// </summary>
        IQuoteActions Actions { get; }
    }
}
