using Lykke.AlgoStore.Algo.Indicators;
using NUnit.Framework;
using System;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MacdFunctionTests
    {
        private const int DEFAULT_PERIOD_SLOW = 2;
        private const int DEFAULT_PERIOD_FAST = 1;
        private const int DEFAULT_PERIOD_SIGNAL = 2;

        private static readonly double[] FixedPriceValues = { 1, 2, 3 };

        private MACD DefaultMacd => new MACD(
            DEFAULT_PERIOD_FAST,
            DEFAULT_PERIOD_SLOW,
            DEFAULT_PERIOD_SLOW,
            default(DateTime),
            default(DateTime),
            Models.Enumerators.CandleTimeInterval.Unspecified,
            "",
            AlgoStore.Algo.CandleOperationMode.CLOSE);

        [Test]
        public void CalculateMacd_ForNullInput_ThrowsException()
        {
            var function = DefaultMacd;

            double[] values = null;

            Assert.Throws<ArgumentNullException>(() => function.WarmUp(values));
        }

        [Test]
        public void CalculateMacd_ForGivenValues_ReturnsCorrectResult()
        {
            var function = DefaultMacd;

            // Temporary, should be removed once the properties become immutable
            function.FastEmaPeriod = DEFAULT_PERIOD_FAST;
            function.SlowEmaPeriod = DEFAULT_PERIOD_SLOW;
            function.SignalLinePeriod = DEFAULT_PERIOD_SIGNAL;

            var result = function.WarmUp(FixedPriceValues);

            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void CalculateMacd_ForPartialWarmupThenAddRest_ReturnsCorrectResult()
        {
            var function = DefaultMacd;

            // Temporary, should be removed once the properties become immutable
            function.FastEmaPeriod = DEFAULT_PERIOD_FAST;
            function.SlowEmaPeriod = DEFAULT_PERIOD_SLOW;
            function.SignalLinePeriod = DEFAULT_PERIOD_SIGNAL;

            function.WarmUp(FixedPriceValues.Take(1).ToArray());
            function.AddNewValue(FixedPriceValues[1]);
            var result = function.AddNewValue(FixedPriceValues[2]);

            Assert.AreEqual(0.5, result);
        }
    }
}
