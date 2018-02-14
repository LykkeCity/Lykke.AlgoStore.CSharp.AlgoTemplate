using System;
using System.Collections.Generic;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IFunctionsService"/>
    /// </summary>
    public class FunctionsService : IFunctionsService
    {
        /// <summary>
        /// <see cref="IFunctionsResultsProvider"/> implementation for feeding the function
        /// results. This implementation is creating a shallow copy of the provided values
        /// </summary>
        public class ShallowCopyFunctionsResults : IFunctionsResultsProvider
        {
            private Dictionary<string, object> _results = new Dictionary<string, object>();

            /// <summary>
            /// Initializes new instance of <see cref="ShallowCopyFunctionsResults"/> with the
            /// a given results.
            /// </summary>
            /// <param name="results">Dictionary of function instance ids and the corresponding 
            /// values</param>
            public ShallowCopyFunctionsResults(Dictionary<string, object> results)
            {
                foreach (var latestResult in results)
                {
                    _results[latestResult.Key] = latestResult.Value;
                }
            }

            public object GetValue(string functionInstanceId)
            {
                return _results[functionInstanceId];
            }
        }

        // Dependencies:
        private IFunctionInitializationService _functionInitializationService;

        // Fields:
        private Dictionary<string, object> _latestFunctionResults;
        private Dictionary<string, IFunction> _allFunctions;

        /// <summary>
        /// Initializes new instance of <see cref="FunctionsService"/>
        /// </summary>
        /// <param name="functionInitializationService"><see cref="IFunctionInitializationService"/> 
        /// implementation for accessing the function instances</param>
        public FunctionsService(IFunctionInitializationService functionInitializationService)
        {
            _functionInitializationService = functionInitializationService;
        }

        public void Initialize()
        {
            // Initialize/Re-initialize the function instances and results
            _allFunctions = new Dictionary<string, IFunction>();
            _latestFunctionResults = new Dictionary<string, object>();

            foreach (var function in _functionInitializationService.GetAllFunctions())
            {
                // Create a mapping between function instance id and function instance;
                _allFunctions[function.FunctionParameters.FunctionInstanceIdentifier] = function;
                _latestFunctionResults[function.FunctionParameters.FunctionInstanceIdentifier] = null;
            }
        }

        public IEnumerable<CandleServiceRequest> GetCandleRequests()
        {
            foreach (var function in _allFunctions.Values)
            {
                yield return new CandleServiceRequest()
                {
                    RequestId = function.FunctionParameters.FunctionInstanceIdentifier,
                    AssetPair = function.FunctionParameters.AssetPair,
                    CandleInterval = function.FunctionParameters.CandleTimeInterval,
                    StartFrom = function.FunctionParameters.StartingDate
                };
            }
        }

        public void WarmUp(IList<MultipleCandlesResponse> functionCandles)
        {
            if (functionCandles == null)
            {
                return;
            }

            lock (_latestFunctionResults)
            {
                foreach (var functionCandle in functionCandles)
                {
                    if (_allFunctions.ContainsKey(functionCandle.RequestId))
                    {
                        var functionId = functionCandle.RequestId;

                        var newFunctionValue = _allFunctions[functionId].WarmUp(functionCandle.Candles);
                        _latestFunctionResults[functionId] = newFunctionValue;
                    }
                }
            }
        }

        public void Recalculate(IList<SingleCandleResponse> candles)
        {
            if (candles == null)
            {
                return;
            }

            // Lock the _latestFunctionResults so that if an algo run is scheduled it will wait
            // for all function to have new values and not some of them
            lock (_latestFunctionResults)
            {
                foreach (var candlesResponse in candles)
                {
                    if (_allFunctions.ContainsKey(candlesResponse.RequestId))
                    {
                        var functionId = candlesResponse.RequestId;

                        var newFunctionValue = _allFunctions[functionId].AddNewValue(candlesResponse.Candle);
                        _latestFunctionResults[functionId] = newFunctionValue;
                    }
                }
            }

        }

        public IFunctionsResultsProvider GetFunctionResults()
        {
            lock (_latestFunctionResults)
            {
                return new ShallowCopyFunctionsResults(_latestFunctionResults);
            }
        }

    }
}
