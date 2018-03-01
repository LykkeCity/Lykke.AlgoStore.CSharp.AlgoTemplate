using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class QuoteContext : Context, IQuoteContext
    {
        public IQuoteData Data { get; set; }
    }
}
