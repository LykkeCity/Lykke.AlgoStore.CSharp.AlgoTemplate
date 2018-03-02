using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MACD;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using System;
using System.Collections.Generic;
using System.Text;

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
                    CandleTimeInterval = Algo.Core.Candles.CandleTimeInterval.Day,
                    Capacity = 100
                }),
                new SmaFunction(new SmaParameters(){
                    FunctionInstanceIdentifier = "SMA_Long",
                    CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                    CandleTimeInterval = Algo.Core.Candles.CandleTimeInterval.Day,
                    Capacity = 1000
                }),
                new AdxFunction(new AdxParameters(){
                    FunctionInstanceIdentifier = "ADX",
                    CandleTimeInterval = Algo.Core.Candles.CandleTimeInterval.Day,
                    AdxPeriod = 14
                }),
                new MacdFunction(new MacdParameters(){
                    FunctionInstanceIdentifier = "MACD",
                    CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                    CandleTimeInterval = Algo.Core.Candles.CandleTimeInterval.Day,
                    FastEmaPeriod = 12,
                    SlowEmaPeriod = 26,
                    SignalLinePeriod = 9
                })
            };
        }
    }
}
