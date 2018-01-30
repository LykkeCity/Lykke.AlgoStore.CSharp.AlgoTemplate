using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IStatisticsService : IData
    {
        void OnQuote(IAlgoQuote quote);
        void OnAction(bool isBuy, double volume);
    }
}
