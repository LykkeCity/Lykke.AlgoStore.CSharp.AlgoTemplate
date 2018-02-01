using System;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class SmaFunctionTests
    {
        private static double[] CustomPriceValues => new Random().GenerateRandomDoubles(100, 164, 168, 4).ToArray();
        private static readonly double[] FixedPriceValues = { 11,12,13,14,15,16,17 };

        [Test]
        public void CalculateSmaForEmptyInputAndOneAddedValueReturnsThatValue()
        {
            var values = new double[] { };
            var function = new SmaFunction();

            function.WarmUp(values);
            function.AddNewValue(11);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 11);
        }

        [Test]
        public void CalculateSmaForEmptyInputAndTwoAddedValueReturnsAverageValue()
        {
            var values = new double[] { };
            var function = new SmaFunction();

            function.WarmUp(values);
            function.AddNewValue(11);
            function.AddNewValue(12);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 11.5);
        }

        [Test]
        public void CalculateSmaForEmptyInputAndThreeAddedValueReturnsAverageValue()
        {
            var values = new double[] { };
            var function = new SmaFunction();

            function.WarmUp(values);
            function.AddNewValue(11);
            function.AddNewValue(12);
            function.AddNewValue(13);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 12);
        }

        [Test]
        public void CalculateSmaForEmptyInputReturnsZero()
        {
            var values = new double[] { };
            var function = new SmaFunction();

            function.WarmUp(values);
            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 0);
        }

        [Test]
        public void CalculateSmaForOneValueInputReturnsThatValue()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction();

            function.WarmUp(values);
            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, values[0]);
        }

        [Test]
        public void CalculateSmaForTwoValuesInputReturnsAverageValue()
        {
            var values = FixedPriceValues.Take(2).ToArray();
            var function = new SmaFunction();

            function.WarmUp(values);
            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 11.5);
        }

        [Test]
        public void CalculateSmaForThreeValuesInputReturnsAverageValue()
        {
            var values = FixedPriceValues.Take(3).ToArray();
            var function = new SmaFunction();

            function.WarmUp(values);
            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 12);
        }

        [Test]
        public void CalculateSmaTestForFixedData()
        {
            var smaAlgo = new SmaAlgo
            {
                Parameters = new SmaParameters
                {
                    LongTermPeriod = 5,
                    ShortTermPeriod = 3,
                    Decimals = 4
                }
            };

            smaAlgo.WarmUp(FixedPriceValues);

            Assert.AreEqual(smaAlgo.GetLongTermSmaValue(), 15);
            Assert.AreEqual(smaAlgo.GetShortTermSmaValue(), 16);
        }

        [Test]
        public void CalculateSmaForOneValueInputAndOneAddedValueReturnsThatNewlyAddedValue()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction();

            function.WarmUp(values);
            function.AddNewValue(99);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 99);
        }

        [Test]
        public void CalculateSmaForOneValueInputAndMultipleAddedValuesReturnsLastAddedValue()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction();

            function.WarmUp(values);
            function.AddNewValue(99);
            function.AddNewValue(88);
            function.AddNewValue(77);
            function.AddNewValue(66);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 66);
        }

        [Test, Explicit("Run manually only cause it uses random generated data for calculus")]
        public void CalculateSmaTestForRandomData()
        {
            var smaAlgo = new SmaAlgo
            {
                Parameters = new SmaParameters
                {
                    LongTermPeriod = 50,
                    ShortTermPeriod = 20,
                    Decimals = 4
                }
            };

            smaAlgo.WarmUp(CustomPriceValues);

            Assert.Greater(smaAlgo.GetLongTermSmaValue(), 164);
            Assert.Greater(smaAlgo.GetShortTermSmaValue(), 164);
        }
    }

}
