using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class AdxFunctionTests
    {
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
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Trim() == "ADX 14")
                        {
                            break;
                        }
                    }
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

        private readonly string _fileNameCorrectData = "Adx_Examples2.txt";
        public readonly string _fileNameNotFullData = "Adx_Examples.txt";

        private ADX DefaultAdx => new ADX(
            DEFAULT_PERCISION,
            default(DateTime),
            default(DateTime),
            Models.Enumerators.CandleTimeInterval.Unspecified,
            "");

        private ATR DefaultAtr => new ATR(
            DEFAULT_PERCISION,
            default(DateTime),
            default(DateTime),
            Models.Enumerators.CandleTimeInterval.Unspecified,
            "");

        private DMI DefaultDmi => new DMI(
            DEFAULT_PERCISION,
            default(DateTime),
            default(DateTime),
            Models.Enumerators.CandleTimeInterval.Unspecified,
            "");

        [Test]
        public void CalculateAdxWarmUp()
        {
            var function = DefaultAdx;
            var values = GetTestCandles("Adx_Examples2.txt");

            double? adxValue = 0.0d;
            double valueToCheck = 20.39;

            adxValue = function.WarmUp(values);

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueToCheck, Math.Round(adxValue.Value, 2));
        }

        [Test]
        public void CalculateAdxWarmUpBiggerData()
        {
            var function = DefaultAdx;
            var values = GetTestCandles("Adx_Examples3.txt");

            double? adxValue = 0.0d;
            double valueToCheck = 16.71;

            adxValue = function.WarmUp(values);

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueToCheck, Math.Round(adxValue.Value, 2));
            Assert.AreEqual(adxValue, function.Value);

        }

        [Test]
        public void CalculateAdxWarmUpAndAddTheRest()
        {
            var function = DefaultAdx;
            var values = GetTestCandles("Adx_Examples2.txt").ToList();

            var valuesToWarmUp = values.GetRange(0, 28);
            var valuesToAdd = values.Skip(28).ToList();

            double? adxValue = 0.0d;
            double valueFirstAdxToCheck = 20.96;
            double valueFinalAdxToCheck = 20.39;

            adxValue = function.WarmUp(valuesToWarmUp);

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueFirstAdxToCheck, Math.Round(adxValue.Value, 2));

            foreach (var val in valuesToAdd)
            {
                adxValue = function.AddNewValue(val);
            }

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueFinalAdxToCheck, Math.Round(adxValue.Value, 2));
        }

        [Test]
        public void CalculateAdxWarmUpReturnNull()
        {
            var function = DefaultAdx;
            var values = GetTestCandles(_fileNameNotFullData);

            double? adxValue = 0.0d;

            adxValue = function.WarmUp(values);
            Assert.IsNull(adxValue);
        }

        [Test]
        public void CalculateAdxAddNew()
        {
            var function = DefaultAdx;
            var values = GetTestCandles(_fileNameCorrectData);

            double? adxValue = 0.0d;
            double valueToCheck = 20.39;
            foreach (var val in values)
            {
                adxValue = function.AddNewValue(val);
            }

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueToCheck, Math.Round(adxValue.Value, 2));
        }

        [Test]
        public void CalculateATRFunction()
        {
            var function = DefaultAtr;
            var values = GetTestCandles(_fileNameCorrectData);

            var atrValue = function.WarmUp(values);
            double valueToCheck = 0.82;

            Assert.NotNull(atrValue);
            Assert.AreEqual(valueToCheck, Math.Round(atrValue.Value, 2));
        }

        [Test]
        public void CalculateATRFunctionAddNew()
        {
            var function = DefaultAtr;
            var values = GetTestCandles(_fileNameCorrectData);

            double? atrValue = 0.0d;

            foreach (var val in values)
            {
                atrValue = function.AddNewValue(val);
            }
            double valueToCheck = 0.82;

            Assert.NotNull(atrValue);
            Assert.AreEqual(valueToCheck, Math.Round(atrValue.Value, 2));
        }

        [Test]
        public void CalculateDMIPlusFunction()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameCorrectData);

            function.WarmUp(values);
            var dmiPlusValue = function.UpwardDirectionalIndex;

            double valueToCheck = 19.26;

            Assert.NotNull(dmiPlusValue);
            Assert.AreEqual(valueToCheck, Math.Round(dmiPlusValue.Value, 2));
        }

        [Test]
        public void CalculateDMIPlusFunctionReturnNull()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameNotFullData);

            function.WarmUp(values);
            Assert.AreEqual(0.0d, function.UpwardDirectionalIndex.Value);
        }

        [Test]
        public void CalculateDMIPlusFunctionAddNew()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameCorrectData);

            double? dmiPlusValue = 0.0d;

            foreach (var val in values)
            {
                dmiPlusValue = function.AddNewValue(val);
            }

            double valueToCheck = 19.26;

            Assert.NotNull(dmiPlusValue);
            Assert.AreEqual(valueToCheck, Math.Round(function.UpwardDirectionalIndex.Value, 2));
        }

        [Test]
        public void CalculateDMIPlusFunctionWarmUpAndAddTheRest()
        {
            var function = DefaultDmi;
            var values = GetTestCandles("Adx_Examples2.txt").ToList();

            var valuesToWarmUp = values.GetRange(0, 15);
            var valuesToAdd = values.Skip(15).ToList();

            double? dmiPlus = 0.0d;
            double valueFirstDMIPlusCheck = 25.20;
            double valueFinalDMIPlusToCheck = 19.26;

            dmiPlus = function.WarmUp(valuesToWarmUp);

            Assert.NotNull(dmiPlus);
            Assert.AreEqual(valueFirstDMIPlusCheck, Math.Round(function.UpwardDirectionalIndex.Value, 2));

            foreach (var val in valuesToAdd)
            {
                dmiPlus = function.AddNewValue(val);
            }

            Assert.NotNull(dmiPlus);
            Assert.AreEqual(valueFinalDMIPlusToCheck, Math.Round(function.UpwardDirectionalIndex.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunction()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameCorrectData);

            var dmiMinusValue = function.WarmUp(values);

            double valueToCheck = 29.47;

            Assert.NotNull(dmiMinusValue);
            Assert.AreEqual(valueToCheck, Math.Round(function.DownwardDirectionalIndex.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunctionAddNew()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameCorrectData);

            double? dmiMinusValue = 0.0d;

            foreach (var val in values)
            {
                dmiMinusValue = function.AddNewValue(val);
            }

            double valueToCheck = 29.47;

            Assert.NotNull(dmiMinusValue);
            Assert.AreEqual(valueToCheck, Math.Round(function.DownwardDirectionalIndex.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunctionReturnNull()
        {
            var function = DefaultDmi;
            var values = GetTestCandles(_fileNameNotFullData);

            double? dmiMinus = 0.0d;

            dmiMinus = function.WarmUp(values);
            Assert.AreEqual(0.0d, dmiMinus);
        }

        [Test]
        public void CalculateDMIMinusFunctionWarmUpAndAddTheRest()
        {
            var function = DefaultDmi;
            var values = GetTestCandles("Adx_Examples2.txt").ToList();

            var valuesToWarmUp = values.GetRange(0, 15);
            var valuesToAdd = values.Skip(15).ToList();

            double? dmiMinus = 0.0d;
            double valueFirstDMIMinusCheck = 17.13;
            double valueFinalDMIMinusToCheck = 29.47;

            dmiMinus = function.WarmUp(valuesToWarmUp);

            Assert.NotNull(dmiMinus);
            Assert.AreEqual(valueFirstDMIMinusCheck, Math.Round(function.DownwardDirectionalIndex.Value, 2));

            foreach (var val in valuesToAdd)
            {
                dmiMinus = function.AddNewValue(val);
            }

            Assert.NotNull(dmiMinus);
            Assert.AreEqual(valueFinalDMIMinusToCheck, Math.Round(function.DownwardDirectionalIndex.Value, 2));
        }
    }
}
