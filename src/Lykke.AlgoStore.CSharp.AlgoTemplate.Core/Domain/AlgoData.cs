using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{

    /// <summary>
    /// <see cref="IData"/> implementation of the algo data
    /// </summary>
    public class AlgoData : IData
    {
        private IAlgoQuote quote;

        /// <summary>
        /// Initializes new instance of the <see cref="AlgoData"/>
        /// </summary>
        /// <param name="quote">The <see cref="IAlgoQuote"/> implementation</param>
        public AlgoData(IAlgoQuote quote)
        {
            this.quote = quote;
        }

        public IAlgoQuote GetQuote()
        {
            return this.quote;
        }
    }
}
