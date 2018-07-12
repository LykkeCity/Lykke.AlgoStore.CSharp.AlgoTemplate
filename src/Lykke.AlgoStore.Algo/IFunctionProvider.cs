using Lykke.AlgoStore.Algo.Indicators;

namespace Lykke.AlgoStore.Algo
{
    /// <summary>
    /// Interface for accessing function results
    /// </summary>
    public interface IFunctionProvider
    {
        T GetFunction<T>(string functionName) where T : IIndicator;
        IIndicator GetFunction(string functionName);
        double? GetValue(string functionName);
    }
}
