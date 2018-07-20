using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                    DateTime = DateTime.UtcNow,
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
                .Returns(Mock.Of<ICandleActions>());

            return candleContextMock.Object;
        }

        [Test]
        public void MovingAverageCrossInitial()
        {
            var mockedContext = CreateContextMock();
            var algo = new MovingAverageCrossAlgo();

            SMA smaShort = null;
            SMA smaLong = null;
            ADX adx = null;

            var candles = GetTestCandles(_fileNameCorrectData);

            SetUpAlgo(algo, (sma) =>
            {
                smaShort = sma;
                smaShort.WarmUp(candles.Select(c => c.Close).ToArray());
            }, (sma) =>
            {
                smaLong = sma;
                smaLong.WarmUp(candles.Select(c => c.Close).ToArray());
            }, (a) =>
            {
                adx = a;
                adx.WarmUp(candles);
            });

            algo.OnStartUp();

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

            SMA smaShort = null;
            SMA smaLong = null;
            ADX adx = null;

            var candles = GetTestCandles(_fileNameCorrectData);

            SetUpAlgo(algo, (sma) =>
            {
                smaShort = sma;
                smaShort.WarmUp(candles.Select(c => c.Close).ToArray());
            }, (sma) =>
            {
                smaLong = sma;
                smaLong.WarmUp(candles.Select(c => c.Close).ToArray());
            }, (a) =>
            {
                adx = a;
                adx.WarmUp(candles);
            });

            algo.OnStartUp();

            Assert.AreEqual(20.96, Math.Round(algo.GetADX().Value, 2));
            Assert.AreEqual(45.85, Math.Round(algo.GetSMAShortTerm(), 2));
            Assert.AreEqual(45.60, Math.Round(algo.GetSMALongTerm(), 2));
            Assert.AreEqual(false, algo.GetCrossSMAShortBelow());
            Assert.AreEqual(false, algo.GetCrossSMAShortAbove());

            adx.AddNewValue(new Candle()
            {
                DateTime = DateTime.UtcNow,
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
                DateTime = DateTime.UtcNow,
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

        private void SetUpAlgo(
            MovingAverageCrossAlgo algo,
            Action<SMA> shortCallback,
            Action<SMA> longCallback,
            Action<ADX> adxCallback)
        {
            var field = algo.GetType()
                            .BaseType
                            .GetField("_paramProvider", BindingFlags.Instance | BindingFlags.NonPublic);

            var paramProviderMock = new Mock<IIndicatorManager>(MockBehavior.Strict);

            paramProviderMock.Setup(m => m.GetParam<DateTime>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(default(DateTime));

            paramProviderMock.Setup(m => m.GetParam<CandleTimeInterval>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(CandleTimeInterval.Day);

            paramProviderMock.Setup(m => m.GetParam<CandleOperationMode>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(CandleOperationMode.CLOSE);

            paramProviderMock.Setup(m => m.GetParam<string>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("");

            paramProviderMock.Setup(m => m.GetParam<int>("SMA_Short", "period"))
                .Returns(10);

            paramProviderMock.Setup(m => m.GetParam<int>("SMA_Long", "period"))
                .Returns(30);

            paramProviderMock.Setup(m => m.GetParam<int>("ADX", "period"))
                .Returns(14);

            paramProviderMock.Setup(m => m.RegisterIndicator(It.IsAny<string>(), It.IsAny<IIndicator>()))
                .Callback((string name, IIndicator indicator) =>
                {
                    switch(name)
                    {
                        case "SMA_Short":
                            shortCallback((SMA)indicator);
                            break;
                        case "SMA_Long":
                            longCallback((SMA)indicator);
                            break;
                        case "ADX":
                            adxCallback((ADX)indicator);
                            break;
                    }
                });

            field.SetValue(algo, paramProviderMock.Object);
        }
    }
}
