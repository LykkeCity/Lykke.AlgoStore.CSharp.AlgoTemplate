﻿using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MACD;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
            var macd = context.Functions.GetFunction<MacdFunction>("MACD");

            algo.OnStartup(context.Functions);

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

            var macd = new MacdFunction(new MacdParameters
            {
                FastEmaPeriod = 1,
                SlowEmaPeriod = 2,
                SignalLinePeriod = 2
            });

            macd.WarmUp(new double[] { 500, 500, 500, 500, 500, 530 });

            var functionsMock = new Mock<IFunctionProvider>();
            functionsMock.Setup(c => c.GetFunction<MacdFunction>("MACD"))
                .Returns(macd);

            candleContextMock.Setup(c => c.Functions)
                .Returns(functionsMock.Object);

            return candleContextMock.Object;
        }
    }
}