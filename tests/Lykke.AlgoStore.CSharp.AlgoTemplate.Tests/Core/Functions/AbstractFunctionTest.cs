﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Core.Functions
{
    [TestFixture]
    public class AbstractFunctionTest
    {
        private class AbstractFunctionUnderTestImpl : AbstractIndicator
        {
            private double? _latestAddNewValue;
            private double[] _latestWarmUp;
            private double? _latestValue;

            public Action<double[]> WarmupAction { get; set; }
            public Action<double> AddNewValueAction { get; set; }

            public double? LatestAddNewValue => _latestAddNewValue;
            public double[] LatestWarmUp => _latestWarmUp;
            public override double? Value => _latestValue;
            public override bool IsReady => Value != null;

            public AbstractFunctionUnderTestImpl(CandleOperationMode candleValue)
                : base(
                     default(DateTime), 
                     default(DateTime), 
                     Models.Enumerators.CandleTimeInterval.Unspecified,
                     "",
                     candleValue)
            {
            }

            public override double? AddNewValue(double value)
            {
                _latestAddNewValue = value;
                _latestValue = value;

                AddNewValueAction?.Invoke(value);

                return value;
            }

            public override double? WarmUp(IEnumerable<double> values)
            {
                var valuesArray = values.ToArray();

                _latestWarmUp = valuesArray;
                _latestValue = Value;

                WarmupAction?.Invoke(valuesArray);

                return valuesArray.Length;
            }
        }


        [Test]
        public void WarmUp_CandleValues_Open()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN);
            var warmupData = CreateCandlesList(2);

            function.WarmUp(warmupData);

            var actualWarmedWith = function.LatestWarmUp;

            Assert.AreEqual(2, actualWarmedWith.Length);
            Assert.AreEqual(warmupData[0].Open, actualWarmedWith[0]);
            Assert.AreEqual(warmupData[1].Open, actualWarmedWith[1]);
        }

        [Test]
        public void WarmUp_CandleValues_Close()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.CLOSE);
            var warmupData = CreateCandlesList(2);

            function.WarmUp(warmupData);

            var actualWarmedWith = function.LatestWarmUp;

            Assert.AreEqual(2, actualWarmedWith.Length);
            Assert.AreEqual(warmupData[0].Close, actualWarmedWith[0]);
            Assert.AreEqual(warmupData[1].Close, actualWarmedWith[1]);
        }

        [Test]
        public void WarmUp_CandleValues_Low()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.LOW);
            var warmupData = CreateCandlesList(2);

            function.WarmUp(warmupData);

            var actualWarmedWith = function.LatestWarmUp;

            Assert.AreEqual(2, actualWarmedWith.Length);
            Assert.AreEqual(warmupData[0].Low, actualWarmedWith[0]);
            Assert.AreEqual(warmupData[1].Low, actualWarmedWith[1]);
        }

        [Test]
        public void WarmUp_CandleValues_High()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.HIGH);
            var warmupData = CreateCandlesList(2);

            function.WarmUp(warmupData);

            var actualWarmedWith = function.LatestWarmUp;

            Assert.AreEqual(2, actualWarmedWith.Length);
            Assert.AreEqual(warmupData[0].High, actualWarmedWith[0]);
            Assert.AreEqual(warmupData[1].High, actualWarmedWith[1]);
        }

        [Test]
        public void WarmUp_CanInvokeWithAllCandleValues()
        {
            var allSupportedCandleValues = Enum.GetValues(typeof(CandleOperationMode))
                .Cast<CandleOperationMode>();

            var warmupData = CreateCandlesList(2);

            foreach (var candleValue in allSupportedCandleValues)
            {
                var function = new AbstractFunctionUnderTestImpl(candleValue);
                function.WarmUp(warmupData);
                var actualWarmedWith = function.LatestWarmUp;
                Assert.AreEqual(2, actualWarmedWith.Length);
            }
        }

        [Test]
        public void WarmUp_WhenInvokedWith_EmptyList()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN);
            var warmupData = new List<Candle>();

            function.WarmUp(warmupData);

            var actualWarmedWith = function.LatestWarmUp;
            Assert.AreEqual(0, actualWarmedWith.Length);
        }

        [Test]
        public void WarmUp_WhenInvokedWith_Null()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN);

            function.WarmUp((List<Candle>)null);

            var actualWarmedWith = function.LatestWarmUp;
            Assert.AreEqual(0, actualWarmedWith.Length);
        }

        [Test]
        public void AddNewValue_WhenExceptionIsThrownInWarmupLogic()
        {
            var exceptionThrownWhileAddingNewValue =
                new Exception("Some exception is thrown during warm-up");
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN)
            {
                WarmupAction = x => throw exceptionThrownWhileAddingNewValue
            };

            var invokedWith = new List<Candle>();
            var ex = Assert.Throws<AbstractIndicator.WarmUpException>(() => function.WarmUp(invokedWith));
            Assert.That(ex.Message, Is.EqualTo("Exception thrown while warming up function 'AbstractFunctionUnderTestImpl'"));
            Assert.That(ex.ValueInvokedWith, Is.EqualTo(invokedWith));
            Assert.That(ex.InnerException, Is.EqualTo(exceptionThrownWhileAddingNewValue));
        }

        [Test]
        public void AddNewValue_CandleValues_Open()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN);
            var addNewValueData = CreateCandle();

            function.AddNewValue(addNewValueData);

            Assert.AreEqual(addNewValueData.Open, function.LatestAddNewValue);
        }

        [Test]
        public void AddNewValue_CandleValues_Close()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.CLOSE);
            var addNewValueData = CreateCandle();

            function.AddNewValue(addNewValueData);

            Assert.AreEqual(addNewValueData.Close, function.LatestAddNewValue);
        }

        [Test]
        public void AddNewValue_CandleValues_Low()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.LOW);
            var addNewValueData = CreateCandle();

            function.AddNewValue(addNewValueData);

            Assert.AreEqual(addNewValueData.Low, function.LatestAddNewValue);
        }

        [Test]
        public void AddNewValue_CandleValues_High()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.HIGH);
            var addNewValueData = CreateCandle();

            function.AddNewValue(addNewValueData);

            Assert.AreEqual(addNewValueData.High, function.LatestAddNewValue);
        }

        [Test]
        public void AddNewValue_CanInvokeWithAllCandleValues()
        {
            var allSupportedCandleValues = Enum.GetValues(typeof(CandleOperationMode))
                .Cast<CandleOperationMode>();

            var addNewValueData = CreateCandle();

            foreach (var candleValue in allSupportedCandleValues)
            {
                var function = new AbstractFunctionUnderTestImpl(candleValue);
                function.AddNewValue(addNewValueData);
                Assert.NotNull(function.LatestAddNewValue);
            }
        }

        [Test]
        public void AddNewValue_WhenInvokedWith_Null()
        {
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN);

            var ex = Assert.Throws<AbstractIndicator.AddNewValueException>(() => function.AddNewValue(null));
            Assert.That(ex.Message, Is.EqualTo("Invalid value of null provided for add new value"));
            Assert.That(ex.ValueInvokedWith, Is.EqualTo(null));
        }


        [Test]
        public void AddNewValue_WhenExceptionIsThrownInAddNewValueLogic()
        {
            var exceptionThrownWhileAddingNewValue =
                new Exception("Some exception is thrown during add new value evaluation");
            var function = new AbstractFunctionUnderTestImpl(CandleOperationMode.OPEN)
            {
                AddNewValueAction = x => throw exceptionThrownWhileAddingNewValue
            };

            var invokedWith = new Candle();
            var ex = Assert.Throws<AbstractIndicator.AddNewValueException>(() => function.AddNewValue(invokedWith));
            Assert.That(ex.Message, Is.EqualTo("Exception thrown while adding new value for a function 'AbstractFunctionUnderTestImpl'"));
            Assert.That(ex.ValueInvokedWith, Is.EqualTo(invokedWith));
            Assert.That(ex.InnerException, Is.EqualTo(exceptionThrownWhileAddingNewValue));
        }

        private Candle CreateCandle()
        {
            return new Candle()
            {
                Open = 1,
                Close = 1000,
                Low = 10_000,
                High = 100_000,
            };
        }

        private List<Candle> CreateCandlesList(int size)
        {
            var result = new List<Candle>();

            for (var i = 0; i < size; i++)
            {
                result.Add(new Candle
                {
                    Open = 1 + i,
                    Close = 1000 + i,
                    Low = 10_000 + i,
                    High = 100_000 + i,
                });
            }

            return result;
        }
    }
}
