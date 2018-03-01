using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.Algo.Implemention.MovingAverageCross;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MovingAverageCrossAlgoTest
    {
        private readonly string _fileNameCorrectData = "MACAlgo_Data.txt";

        private const int DEFAULT_PERCISION = 14;

        public IList<Candle> GetTestCandles(string externalDataFilename)
        {
            List<Candle> candles = new List<Candle>();

            bool first = true;
            foreach (var line in File.ReadLines(Path.Combine("TestData", externalDataFilename)))
            {
                var parts = line.Split(',');
                if (first)
                {
                    continue;
                }

                candles.Add(new Candle
                {
                    DateTime = DateTime.Now,
                    Open = 0,
                    High = Convert.ToDouble(parts[0]),
                    Low = Convert.ToDouble(parts[1]),
                    Close = Convert.ToDouble(parts[2]),
                });
            }
            return candles;
        }



        private ICandleContext CreateContextMock()
        {
            var candleContextMock = new Mock<ICandleContext>();

            candleContextMock.SetupGet(c => c.Actions)
                .Returns(Mock.Of<IActions>());

            var smaShort = new SmaFunction(new SmaParameters
            {
                Capacity = 100,
                CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                CandleTimeInterval = CandleTimeInterval.Day,
            });

            var smaLong = new SmaFunction(new SmaParameters
            {
                Capacity = 1000,
                CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                CandleTimeInterval = CandleTimeInterval.Day,
            });

            var adx = new AdxFunction(new AdxParameters
            {
                CandleTimeInterval = CandleTimeInterval.Day,
                AdxPeriod = 14
            });

            var candles = GetTestCandles(_fileNameCorrectData);

            smaShort.WarmUp(candles.Select(c => c.Close).ToArray());
            smaLong.WarmUp(candles.Select(c => c.Close).ToArray());
            adx.WarmUp(candles);

            var functionsMock = new Mock<IFunctionProvider>();
            functionsMock.Setup(c => c.GetFunction<SmaFunction>("SMA_Short"))
                .Returns(smaShort);

            functionsMock.Setup(c => c.GetFunction<SmaFunction>("SMA_Long"))
               .Returns(smaLong);

            functionsMock.Setup(c => c.GetFunction<AdxFunction>("ADX"))
               .Returns(adx);

            candleContextMock.Setup(c => c.Functions)
                .Returns(functionsMock.Object);

            return candleContextMock.Object;
        }

        [Test]
        public void MovingAverageCrossInitial()
        {
            var mockedContext = CreateContextMock();
            var algo = new MovingAverageCrossAlgo();

            algo.OnStartUp(mockedContext.Functions);

            Assert.IsNull(algo.GetADX());

            double? adxValue = 0.0d;
            double valueToCheck = 20.39;

            //function.OnCandleReceived(MockContext(values[0]));

            //function.WarmUp(values);

        }
    }
}
