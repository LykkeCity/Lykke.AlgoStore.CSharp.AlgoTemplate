using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Moq;
using NUnit.Framework;
using System;
using System.Reflection;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MacdTrendTests
    {
        [Test]
        public void MacdTrend_FunctionsCorrectly_GivenProperData()
        {
            var context = CreateContextMock();
            var algo = new MacdTrendAlgo();
            MACD macd = null;

            SetUpAlgo(algo, (m) => macd = m);

            algo.HoldingStep = 2;
            algo.Tolerance = 0.0025;

            algo.OnStartUp();

            algo.OnCandleReceived(context);
            Assert.AreEqual(2, algo.Holdings);

            macd.AddNewValue(530);
            algo.OnCandleReceived(context);
            Assert.AreEqual(2, algo.Holdings);

            macd.AddNewValue(100);
            algo.OnCandleReceived(context);
            Assert.AreEqual(0, algo.Holdings);
        }

        private ICandleContext CreateContextMock()
        {
            var candleContextMock = new Mock<ICandleContext>();

            candleContextMock.SetupGet(c => c.Actions)
                .Returns(Mock.Of<IActions>());

            return candleContextMock.Object;
        }

        private void SetUpAlgo(MacdTrendAlgo algo, Action<MACD> macdCallback)
        {
            var field = algo.GetType()
                            .BaseType
                            .GetField("_paramProvider", BindingFlags.Instance | BindingFlags.NonPublic);

            var paramProviderMock = new Mock<IIndicatorManager>(MockBehavior.Strict);

            paramProviderMock.Setup(m => m.GetParam<DateTime>("MACD", It.IsAny<string>()))
                .Returns(default(DateTime));

            paramProviderMock.Setup(m => m.GetParam<CandleTimeInterval>("MACD", It.IsAny<string>()))
                .Returns(default(CandleTimeInterval));

            paramProviderMock.Setup(m => m.GetParam<CandleOperationMode>("MACD", It.IsAny<string>()))
                .Returns(default(CandleOperationMode));

            paramProviderMock.Setup(m => m.GetParam<string>("MACD", It.IsAny<string>()))
                .Returns("");

            paramProviderMock.Setup(m => m.GetParam<int>("MACD", "fastEmaPeriod"))
                .Returns(1);

            paramProviderMock.Setup(m => m.GetParam<int>("MACD", "slowEmaPeriod"))
                .Returns(2);

            paramProviderMock.Setup(m => m.GetParam<int>("MACD", "signalLinePeriod"))
                .Returns(2);

            paramProviderMock.Setup(m => m.RegisterIndicator("MACD", It.IsAny<IIndicator>()))
                .Callback((string name, IIndicator indicator) =>
                {
                    var macd = (MACD)indicator;

                    macd.WarmUp(new double[] { 500, 500, 500, 500, 500, 530 });
                    macdCallback(macd);
                });

            field.SetValue(algo, paramProviderMock.Object);
        }
    }
}
