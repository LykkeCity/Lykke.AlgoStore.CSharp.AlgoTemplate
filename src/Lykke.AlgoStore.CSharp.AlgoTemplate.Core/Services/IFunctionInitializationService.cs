using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Text;

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
