﻿namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// Interface for accessing function results
    /// </summary>
    public interface IFunctionsResultsProvider
    {
        object GetValue(string functionName);
    }
}
