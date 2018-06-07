using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.ADX;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.MACD;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// A <see cref="IFunctionInitializationService"/> implementation providing 2
    /// Simple moving average functions. This implementation will be replaced by
    /// the generic <see cref="FunctionInitializationService"/> implementation
    /// once the ability to pass parameters and functions to an algo is 
    /// implemented
    /// </summary>
    public class HardCodedMovingAverageCrossFunctionInitializationService : IFunctionInitializationService
    {
        public IList<IFunction> GetAllFunctions()
        {
            return new List<IFunction>()
            {
                new SmaFunction(new SmaParameters(){
                    FunctionInstanceIdentifier = "SMA_Short",
                    CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                    CandleTimeInterval = CandleTimeInterval.Minute,
                    Capacity = 100,
                    AssetPair = "BTCUSD",
                }),
                new SmaFunction(new SmaParameters(){
                    FunctionInstanceIdentifier = "SMA_Long",
                    CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                    CandleTimeInterval = CandleTimeInterval.Minute,
                    Capacity = 1000,
                    AssetPair = "BTCUSD",
                }),
                new AdxFunction(new AdxParameters(){
                    FunctionInstanceIdentifier = "ADX",
                    CandleTimeInterval = CandleTimeInterval.Minute,
                    AdxPeriod = 14,
                    AssetPair = "BTCUSD"
                }),
                new MacdFunction(new MacdParameters(){
                    FunctionInstanceIdentifier = "MACD",
                    CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                    CandleTimeInterval = CandleTimeInterval.Minute,
                    FastEmaPeriod = 12,
                    SlowEmaPeriod = 26,
                    SignalLinePeriod = 9,
                    AssetPair = "BTCUSD"
                })
            };
        }
    }
}
