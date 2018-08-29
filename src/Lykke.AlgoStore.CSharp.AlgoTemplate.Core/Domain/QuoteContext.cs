using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class QuoteContext : Context, IQuoteContext
    {
        public IQuoteData Data { get; set; }
    }
}
