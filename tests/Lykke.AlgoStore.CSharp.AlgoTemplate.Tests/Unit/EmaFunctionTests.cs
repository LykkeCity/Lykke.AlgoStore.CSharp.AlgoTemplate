using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.EMA;
using NUnit.Framework;
using System;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class EmaFunctionTests
    {
        private static double[] CustomPriceValues => new Random().GenerateRandomDoubles(100, 164, 168, 4).ToArray();
        private static readonly double[] FixedPriceValues = { 10, 15, 5, 10, 2, 5, 10 };
        private const int DEFAULT_PERIOD = 3;

        [Test]
        public void CalculateEmaForEmptyInputAndReturnNull()
        {
            var values = new double[] { };
            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(null, warmupValue);
            Assert.AreEqual(false, function.IsReady());
        }

        [Test]
        public void CalculateEmaForNullInputAndReturnException()
        {
            double[] values = null;
            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });

            Assert.Throws<ArgumentException>(() => function.WarmUp(values));
        }

        [Test]
        public void CalculateEmaForPeriodCountInputAndReturnEma()
        {
            double[] values = FixedPriceValues.Take(3).ToArray();

            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });
            var warmupValue = function.WarmUp(values);

            Assert.AreEqual(10, warmupValue);
            Assert.AreEqual(true, function.IsReady());
        }

        [Test]
        public void CalculateEmaForPeriodCountAndAddNewValueAndReturnEma()
        {
            double[] values = FixedPriceValues.Take(3).ToArray();

            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });
            var warmupValue = function.WarmUp(values);

            Assert.AreEqual(10, warmupValue);

            var addNewValue = function.AddNewValue(10);
            Assert.AreEqual(10, addNewValue);
            Assert.AreEqual(true, function.IsReady());
        }


        [Test]
        public void CalculateEmaForPeriodCountAndCallGetEmaAndAddValueAndReturnEma()
        {
            double[] values = FixedPriceValues.Take(4).ToArray();

            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });
            var warmupValue = function.WarmUp(values);

            Assert.AreEqual(10, warmupValue);

            var addNewValue = function.GetEmaAndAddValue(2);
            Assert.AreEqual(6, addNewValue);
            Assert.AreEqual(true, function.IsReady());
        }

        [Test]
        public void CalculateEmaForNotAllPeriodValuesAndSeveralTimesNewValues()
        {
            double[] values = FixedPriceValues.Take(1).ToArray();

            var function = new EmaFunction(new EmaParameters() { EmaPeriod = DEFAULT_PERIOD });
            var warmupValue = function.WarmUp(values);

            Assert.AreEqual(null, warmupValue);

            var addNewValue = function.GetEmaAndAddValue(15);
            Assert.AreEqual(null, addNewValue);
            Assert.AreEqual(false, function.IsReady());

            var addNewValue2 = function.GetEmaAndAddValue(5);
            Assert.AreEqual(10, addNewValue2);
            Assert.AreEqual(true, function.IsReady());
        }
    }
}
