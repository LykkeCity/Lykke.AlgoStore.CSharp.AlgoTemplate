using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
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
                    first = false;
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
                Capacity = 10,
                CandleOperationMode = FunctionParamsBase.CandleValue.CLOSE,
                CandleTimeInterval = CandleTimeInterval.Day,
            });

            var smaLong = new SmaFunction(new SmaParameters
            {
                Capacity = 30,
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

            var smaShort = mockedContext.Functions.GetFunction<SmaFunction>("SMA_Short");
            var smaLong = mockedContext.Functions.GetFunction<SmaFunction>("SMA_Long");
            var adx = mockedContext.Functions.GetFunction<AdxFunction>("ADX");


            algo.OnStartUp(mockedContext.Functions);

            Assert.AreEqual(20.96, Math.Round(algo.GetADX().Value, 2));
            Assert.AreEqual(45.85, Math.Round(algo.GetSMAShortTerm(), 2));
            Assert.AreEqual(45.60, Math.Round(algo.GetSMALongTerm(), 2));

            Assert.AreEqual(20.96, Math.Round(adx.Value.Value, 2));
            Assert.AreEqual(45.85, Math.Round(smaShort.Value.Value, 2));
            Assert.AreEqual(45.60, Math.Round(smaLong.Value.Value, 2));

            Assert.AreEqual(false, algo.GetCrossSMAShortBelow());
            Assert.AreEqual(false, algo.GetCrossSMAShortAbove());
        }

        [Test]
        public void MovingAverageCrossAddNewValues()
        {
            var mockedContext = CreateContextMock();
            var algo = new MovingAverageCrossAlgo();

            var smaShort = mockedContext.Functions.GetFunction<SmaFunction>("SMA_Short");
            var smaLong = mockedContext.Functions.GetFunction<SmaFunction>("SMA_Long");
            var adx = mockedContext.Functions.GetFunction<AdxFunction>("ADX");

            algo.OnStartUp(mockedContext.Functions);

            Assert.AreEqual(20.96, Math.Round(algo.GetADX().Value, 2));
            Assert.AreEqual(45.85, Math.Round(algo.GetSMAShortTerm(), 2));
            Assert.AreEqual(45.60, Math.Round(algo.GetSMALongTerm(), 2));
            Assert.AreEqual(false, algo.GetCrossSMAShortBelow());
            Assert.AreEqual(false, algo.GetCrossSMAShortAbove());

            adx.AddNewValue(new Candle()
            {
                DateTime = DateTime.Now,
                Open = 930,
                High = 45.71,
                Low = 45,
                Close = 45.44,
            });

            smaShort.AddNewValue(45.44);
            smaLong.AddNewValue(45.44);

            algo.OnCandleReceived(mockedContext);

            Assert.AreEqual(20.35, Math.Round(adx.Value.Value, 2));
            Assert.AreEqual(45.71, Math.Round(smaShort.Value.Value, 2));
            Assert.AreEqual(45.59, Math.Round(smaLong.Value.Value, 2));
            Assert.AreEqual(false, algo.GetCrossSMAShortBelow());
            Assert.AreEqual(false, algo.GetCrossSMAShortAbove());

            adx.AddNewValue(new Candle()
            {
                DateTime = DateTime.Now,
                Open = 930,
                High = 45.35,
                Low = 44.45,
                Close = 44.75,
            });

            smaShort.AddNewValue(44.75);
            smaLong.AddNewValue(44.75);

            algo.OnCandleReceived(mockedContext);

            Assert.AreEqual(20.39, Math.Round(adx.Value.Value, 2));
            Assert.AreEqual(45.52, Math.Round(smaShort.Value.Value, 2));
            Assert.AreEqual(45.56, Math.Round(smaLong.Value.Value, 2));

            Assert.AreEqual(20.39, Math.Round(algo.GetADX().Value, 2));
            Assert.AreEqual(45.52, Math.Round(algo.GetSMAShortTerm(), 2));
            Assert.AreEqual(45.56, Math.Round(algo.GetSMALongTerm(), 2));

            Assert.AreEqual(false, algo.GetCrossSMAShortAbove());
            Assert.AreEqual(true, algo.GetCrossSMAShortBelow());
        }
    }
}
