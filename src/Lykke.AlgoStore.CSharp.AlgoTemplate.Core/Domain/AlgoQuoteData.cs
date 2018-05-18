using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// <see cref="IQuoteData"/> implementation of the algo quote data
    /// </summary>
    public class AlgoQuoteData : IQuoteData
    {
        public IAlgoQuote Quote { get; }

        /// <summary>
        /// Initializes new instance of the <see cref="AlgoQuoteData"/>
        /// </summary>
        /// <param name="quote">The <see cref="IAlgoQuote"/> implementation</param>
        public AlgoQuoteData(IAlgoQuote quote)
        {
            Quote = quote;
        }
    }
}
