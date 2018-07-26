using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IFunctionsService"/>
    /// </summary>
    public class FunctionsService : IFunctionsService, IFunctionProvider, IIndicatorManager
    {
        // Dependencies:
        private readonly IAlgoSettingsService _algoSettingsService;

        // Fields:
        private readonly AlgoMetaDataInformation _algoMetaData;
        private readonly Dictionary<string, IIndicator> _allFunctions = new Dictionary<string, IIndicator>();
        private readonly Dictionary<string, AlgoMetaDataFunction> _indicatorData 
            = new Dictionary<string, AlgoMetaDataFunction>();

        /// <summary>
        /// Initializes new instance of <see cref="FunctionsService"/>
        /// </summary>
        /// <param name="functionInitializationService"><see cref="IFunctionInitializationService"/> 
        /// implementation for accessing the function instances</param>
        public FunctionsService(
            IAlgoSettingsService algoSettingsService)
        {
            _algoSettingsService = algoSettingsService;
            _algoMetaData = _algoSettingsService.GetAlgoInstance().AlgoMetaDataInformation;

            foreach(var indicator in _algoMetaData.Functions)
                _indicatorData.Add(indicator.Id, indicator);
        }

        // This is now obsolete - indicators are created through BaseAlgo
        public void Initialize()
        {
        }

        public IEnumerable<CandleServiceRequest> GetCandleRequests(string authToken)
        {
            foreach (var kvp in _allFunctions)
            {
                var function = kvp.Value;

                yield return new CandleServiceRequest()
                {
                    AuthToken = authToken,
                    RequestId = kvp.Key,
                    AssetPair = function.AssetPair,
                    CandleInterval = function.CandleTimeInterval,
                    StartFrom = function.StartingDate,
                    EndOn = function.EndingDate
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
            if (candles == null || candles.Any(c => c.Candle == null))
            {
                return;
            }

            foreach (var candlesResponse in candles)
            {
                if (!_allFunctions.ContainsKey(candlesResponse.RequestId))
                    continue;

                var functionId = candlesResponse.RequestId;

                var function = _allFunctions[functionId];

                if (function.StartingDate > candlesResponse.Candle.DateTime ||
                    function.EndingDate < candlesResponse.Candle.DateTime)
                    continue;

                function.AddNewValue(candlesResponse.Candle);
            }
        }

        public IFunctionProvider GetFunctionResults()
        {
            return this;
        }

        public T GetFunction<T>(string functionName) where T : IIndicator
        {
            if (!_allFunctions.ContainsKey(functionName))
                return default(T);

            return (T)_allFunctions[functionName];
        }

        public IIndicator GetFunction(string functionName)
        {
            if (!_allFunctions.ContainsKey(functionName))
                return null;

            return _allFunctions[functionName];
        }

        public double? GetValue(string functionName)
        {
            return GetFunction(functionName)?.Value;
        }

        public T GetParam<T>(string indicator, string param)
        {
            var indicatorData = _indicatorData[indicator];

            if(indicatorData == null)
                throw new KeyNotFoundException($"The indicator \"{indicator}\" doesn't exist");

            var paramData = indicatorData.Parameters.FirstOrDefault(p => p.Key == param);

            if (paramData == null)
                throw new KeyNotFoundException($"The indicator \"{indicator}\" param \"{param}\" doesn't exist");

            var paramType = typeof(T);

            if (paramType.IsEnum)
                return (T)Enum.ToObject(paramType, Convert.ToInt32(paramData.Value));
            else
                return (T)Convert.ChangeType(paramData.Value, paramType);
        }

        public void RegisterIndicator(string name, IIndicator indicator)
        {
            if (_allFunctions.ContainsKey(name))
                throw new InvalidOperationException(
                    $"An indicator with the name \"{name}\" is already registered");

            _allFunctions.Add(name, indicator);
        }
    }
}
