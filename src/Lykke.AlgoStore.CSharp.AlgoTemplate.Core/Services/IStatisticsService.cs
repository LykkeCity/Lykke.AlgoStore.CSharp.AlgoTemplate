using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing statistics capabilities
    /// </summary>
    public interface IStatisticsService : IData, IStatistics
    {
        void OnQuote(IAlgoQuote quote);
        void OnAction(bool isBuy, double volume, double price);
        void OnAlgoStarted();
        void OnAlgoStopped();
    }
}
