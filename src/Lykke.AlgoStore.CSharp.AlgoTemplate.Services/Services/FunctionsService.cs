using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IFunctionsService"/>
    /// </summary>
    public class FunctionsService : IFunctionsService, IFunctionProvider
    {
        // Dependencies:
        private IFunctionInitializationService _functionInitializationService;

        // Fields:
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

            foreach (var function in _functionInitializationService.GetAllFunctions())
            {
                // Create a mapping between function instance id and function instance;
                _allFunctions[function.FunctionParameters.FunctionInstanceIdentifier] = function;
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
                    StartFrom = function.FunctionParameters.StartingDate,
                    EndOn = function.FunctionParameters.EndingDate
                };
            }
        }

        public void WarmUp(IList<MultipleCandlesResponse> functionCandles)
        {
            if (functionCandles == null)
            {
                return;
            }

            foreach (var functionCandle in functionCandles)
            {
                if (!_allFunctions.ContainsKey(functionCandle.RequestId))
                    continue;

                var functionId = functionCandle.RequestId;

                _allFunctions[functionId].WarmUp(functionCandle.Candles);
            }
        }

        public void Recalculate(IList<SingleCandleResponse> candles)
        {
            if (candles == null)
            {
                return;
            }

            foreach (var candlesResponse in candles)
            {
                if (!_allFunctions.ContainsKey(candlesResponse.RequestId))
                    continue;

                var functionId = candlesResponse.RequestId;

                if (_allFunctions[functionId].FunctionParameters.EndingDate < candlesResponse.Candle.DateTime)
                    continue;

                _allFunctions[functionId].AddNewValue(candlesResponse.Candle);
            }
        }

        public IFunctionProvider GetFunctionResults()
        {
            return this;
        }

        public T GetFunction<T>(string functionName) where T : IFunction
        {
            if (!_allFunctions.ContainsKey(functionName))
                return default(T);

            return (T)_allFunctions[functionName];
        }

        public IFunction GetFunction(string functionName)
        {
            if (!_allFunctions.ContainsKey(functionName))
                return null;

            return _allFunctions[functionName];
        }

        public double? GetValue(string functionName)
        {
            return GetFunction(functionName)?.Value;
        }
    }
}
