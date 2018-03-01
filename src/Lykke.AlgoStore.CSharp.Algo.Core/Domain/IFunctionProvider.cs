using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Interface for accessing function results
    /// </summary>
    public interface IFunctionProvider
    {
        T GetFunction<T>(string functionName) where T : IFunction;
        IFunction GetFunction(string functionName);
        double? GetValue(string functionName);
    }
}
