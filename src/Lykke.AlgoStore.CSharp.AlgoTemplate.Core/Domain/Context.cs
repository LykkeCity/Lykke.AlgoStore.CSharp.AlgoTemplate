using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class Context : IContext
    {
        public IData Data { get; set; }
        public IFunctions Functions { get; set; }
        public IActions Actions { get; set; }
        public IStatistics Statistics { get; set; }
    }
}
