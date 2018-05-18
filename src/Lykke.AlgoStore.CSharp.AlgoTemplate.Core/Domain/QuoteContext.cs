using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class QuoteContext : Context, IQuoteContext
    {
        public IQuoteData Data { get; set; }

        public IQuoteActions Actions { get; set; }
    }
}
