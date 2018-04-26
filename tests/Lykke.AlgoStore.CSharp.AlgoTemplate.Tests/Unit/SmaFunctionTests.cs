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
        private static readonly double[] FixedPriceValues = { 11, 12, 13, 14, 15, 16, 17 };
        private const int DEFAULT_PERCISION = 10;


        [Test]
        public void CalculateSmaForEmptyInputAndOneAddedValueReturnsThatValue()
        {
            var values = new double[] { };
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(null, warmupValue);

            // Feeding the function with a new value
            var addNewValue = function.AddNewValue(11);
            Assert.AreEqual(11, addNewValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(11, sma);
        }

        [Test]
        public void CalculateSmaForEmptyInputAndTwoAddedValueReturnsAverageValue()
        {
            var values = new double[] { };
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(null, warmupValue);

            // Feeding the function with a new value
            var addNewValue = function.AddNewValue(11);
            Assert.AreEqual(11, addNewValue);
            addNewValue = function.AddNewValue(12);
            Assert.AreEqual(11.5, addNewValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(sma, 11.5);
        }

        [Test]
        public void CalculateSmaForEmptyInputAndThreeAddedValueReturnsAverageValue()
        {
            var values = new double[] { };
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(null, warmupValue);

            // Feeding the function with a new value
            var addNewValue = function.AddNewValue(11);
            Assert.AreEqual(11, addNewValue);
            addNewValue = function.AddNewValue(12);
            Assert.AreEqual(11.5, addNewValue);
            addNewValue = function.AddNewValue(13);
            Assert.AreEqual(12, addNewValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(sma, 12);
        }

        [Test]
        public void CalculateSmaForEmptyInputReturnsZero()
        {
            var values = new double[] { };
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(null, warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(null, sma);
        }

        [Test]
        public void CalculateSmaForOneValueInputReturnsThatValue()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(values[0], warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(values[0], sma);
        }

        [Test]
        public void CalculateSmaForTwoValuesInputReturnsAverageValue()
        {
            var values = FixedPriceValues.Take(2).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(11.5, warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(11.5, sma);
        }

        [Test]
        public void CalculateSmaForThreeValuesInputReturnsAverageValue()
        {
            var values = FixedPriceValues.Take(3).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(12, warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(12, sma);
        }

        [Test]
        public void CalculateSmaForOneValueInputAndOneAddedValueReturnsAverageValue()
        {
            var percision = 1;
            var values = FixedPriceValues.Take(percision).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(11, warmupValue);

            // Feeding the function with a new value
            var addNewValue = function.AddNewValue(12);
            Assert.AreEqual(11.5, addNewValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(sma, 11.5);
        }

        [Test]
        public void CalculateSmaForOneValueInputAndMultipleAddedValuesReturnsAverageValue()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = DEFAULT_PERCISION });

            function.WarmUp(values);
            function.AddNewValue(12);
            function.AddNewValue(13);
            function.AddNewValue(14);
            function.AddNewValue(15);

            var sma = function.GetSmaValue();

            Assert.AreEqual(sma, 13);
        }

        [Test]
        public void CalculateSma_WhenExceedingPercision_With1_Warmup()
        {
            var values = FixedPriceValues.Take(2).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = 1 });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(values[1], warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(values[1], sma);
        }

        [Test]
        public void CalculateSma_WhenExceedingPercision_With2_Warmup()
        {
            var values = FixedPriceValues.Take(3).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = 2 });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(12.5, warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(12.5, sma);
        }

        [Test]
        public void CalculateSma_WhenExceedingPercision_With3_Warmup()
        {
            var values = FixedPriceValues.Take(4).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = 3 });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(13, warmupValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(13, sma);
        }

        [Test]
        public void CalculateSma_WhenExceedingPercision_AddinNewValues()
        {
            var values = FixedPriceValues.Take(1).ToArray();
            var function = new SmaFunction(new SmaParameters() { Capacity = 3 });

            // Warming up
            var warmupValue = function.WarmUp(values);
            Assert.AreEqual(values[0], warmupValue);

            // Feeding the function with a new value
            var addNewValue = function.AddNewValue(12);
            Assert.AreEqual(11.5, addNewValue);
            addNewValue = function.AddNewValue(13);
            Assert.AreEqual(12, addNewValue);
            addNewValue = function.AddNewValue(14);
            Assert.AreEqual(13, addNewValue);
            addNewValue = function.AddNewValue(15);
            Assert.AreEqual(14, addNewValue);

            var sma = function.GetSmaValue();
            Assert.AreEqual(14, sma);
        }

        [Test]
        public void SmaFunction_AddNewValue_RunsCorrectlyWithoutWarmup()
        {
            var function = new SmaFunction(new SmaParameters() { Capacity = 3 });

            var result = function.AddNewValue(1);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void SmaFunction_Value_ReturnsCorrectlyWithoutWarmup()
        {
            var function = new SmaFunction(new SmaParameters() { Capacity = 3 });

            var result = function.Value;

            Assert.AreEqual(null, result);
        }

    }

}
