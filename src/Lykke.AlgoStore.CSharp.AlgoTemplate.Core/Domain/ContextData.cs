using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class ContextData : IData
    {
        private readonly IAlgoQuote _algoQuote;

        public ContextData(IAlgoQuote algoQuote)
        {
            _algoQuote = algoQuote;
        }

        public IAlgoQuote GetQuote()
        {
            return _algoQuote;
        }
    }
}
