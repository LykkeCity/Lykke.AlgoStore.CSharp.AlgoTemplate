using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
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
