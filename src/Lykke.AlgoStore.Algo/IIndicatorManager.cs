using Lykke.AlgoStore.Algo.Indicators;

namespace Lykke.AlgoStore.Algo
{
    public interface IIndicatorManager
    {
        T GetParam<T>(string indicator, string param);
        void RegisterIndicator(string name, IIndicator indicator);
    }
}
