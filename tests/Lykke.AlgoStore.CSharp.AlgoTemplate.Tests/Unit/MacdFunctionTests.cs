using NUnit.Framework;
using System;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Functions.MACD;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MacdFunctionTests
    {
        private const int DEFAULT_PERIOD_SLOW = 2;
        private const int DEFAULT_PERIOD_FAST = 1;
        private const int DEFAULT_PERIOD_SIGNAL = 2;

        private static readonly double[] FixedPriceValues = { 1, 2, 3 };

        private MacdParameters DefaultMacdParameters => new MacdParameters
        {
            FastEmaPeriod = DEFAULT_PERIOD_FAST,
            SlowEmaPeriod = DEFAULT_PERIOD_SLOW,
            SignalLinePeriod = DEFAULT_PERIOD_SIGNAL
        };

        [Test]
        public void CreateMacd_WithNullParams_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new MacdFunction(null));
        }

        [Test]
        public void CalculateMacd_ForNullInput_ThrowsException()
        {
            var function = new MacdFunction(DefaultMacdParameters);
            double[] values = null;

            Assert.Throws<ArgumentNullException>(() => function.WarmUp(values));
        }

        [Test]
        public void CalculateMacd_ForGivenValues_ReturnsCorrectResult()
        {
            var function = new MacdFunction(DefaultMacdParameters);

            var result = function.WarmUp(FixedPriceValues);

            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void CalculateMacd_ForPartialWarmupThenAddRest_ReturnsCorrectResult()
        {
            var function = new MacdFunction(DefaultMacdParameters);

            function.WarmUp(FixedPriceValues.Take(1).ToArray());
            function.AddNewValue(FixedPriceValues[1]);
            var result = function.AddNewValue(FixedPriceValues[2]);

            Assert.AreEqual(0.5, result);
        }
    }
}
