using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class CandleContext : Context, ICandleContext
    {
        public ICandleData Data { get; set; }

        public ICandleActions Actions { get; set; }
    }
}
