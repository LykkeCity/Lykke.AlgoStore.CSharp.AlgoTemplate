using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service for initialization of functions
    /// </summary>
    public interface IFunctionInitializationService
    {
        /// <summary>
        /// Get all the functions required to run an algo
        /// </summary>
        /// <returns></returns>
        IList<IFunction> GetAllFunctions();
    }
}
