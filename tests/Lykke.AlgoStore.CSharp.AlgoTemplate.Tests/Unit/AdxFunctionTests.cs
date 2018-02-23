using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            int targetIndex = -1;
            bool fileHasVolume = false;
            foreach (var line in File.ReadLines(Path.Combine("TestData", externalDataFilename)))
            {
                var parts = line.Split(',');
                if (first)
                {
                    fileHasVolume = parts[2].Trim() == "Volume";
                    first = false;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Trim() == "ADX 14")
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                    continue;
                }

                candles.Add(new Candle
                {
                    DateTime = DateTime.Now,
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

        [Test]
        public void CalculateAdxWarmUp()
        {
            var function = new AdxFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
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
            var function = new AdxFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
            var values = GetTestCandles("Adx_Examples3.txt");

            double? adxValue = 0.0d;
            double valueToCheck = 16.71;

            adxValue = function.WarmUp(values);

            Assert.NotNull(adxValue);
            Assert.AreEqual(valueToCheck, Math.Round(adxValue.Value, 2));
        }

        [Test]
        public void CalculateAdxWarmUpAndAddTheRest()
        {
            var function = new AdxFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
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
            var function = new AdxFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameNotFullData);

            double? adxValue = 0.0d;

            adxValue = function.WarmUp(values);
            Assert.IsNull(adxValue);
        }

        [Test]
        public void CalculateAdxAddNew()
        {
            var function = new AdxFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
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
            var function = new ATRFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameCorrectData);

            var atrValue = function.WarmUp(values);
            double valueToCheck = 0.82;

            Assert.NotNull(atrValue);
            Assert.AreEqual(valueToCheck, Math.Round(atrValue.Value, 2));
        }

        [Test]
        public void CalculateATRFunctionAddNew()
        {
            var function = new ATRFunction(new AdxParameters() { AdxPriod = DEFAULT_PERCISION });
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
            var function = new DirectionalMovemnetIndexPlusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameCorrectData);

            var dmiPlusValue = function.WarmUp(values);

            double valueToCheck = 19.26;

            Assert.NotNull(dmiPlusValue);
            Assert.AreEqual(valueToCheck, Math.Round(dmiPlusValue.Value, 2));
        }

        [Test]
        public void CalculateDMIPlusFunctionReturnNull()
        {
            var function = new DirectionalMovemnetIndexPlusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameNotFullData);

            double? dmiPlues = 0.0d;

            dmiPlues = function.WarmUp(values);
            Assert.IsNull(dmiPlues);
        }

        [Test]
        public void CalculateDMIPlusFunctionAddNew()
        {
            var function = new DirectionalMovemnetIndexPlusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameCorrectData);

            double? dmiPlusValue = 0.0d;

            foreach (var val in values)
            {
                dmiPlusValue = function.AddNewValue(val);
            }

            double valueToCheck = 19.26;

            Assert.NotNull(dmiPlusValue);
            Assert.AreEqual(valueToCheck, Math.Round(dmiPlusValue.Value, 2));
        }

        [Test]
        public void CalculateDMIPlusFunctionWarmUpAndAddTheRest()
        {
            var function = new DirectionalMovemnetIndexPlusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles("Adx_Examples2.txt").ToList();

            var valuesToWarmUp = values.GetRange(0, 15);
            var valuesToAdd = values.Skip(15).ToList();

            double? dmiPlus = 0.0d;
            double valueFirstDMIPlusCheck = 25.20;
            double valueFinalDMIPlusToCheck = 19.26;

            dmiPlus = function.WarmUp(valuesToWarmUp);

            Assert.NotNull(dmiPlus);
            Assert.AreEqual(valueFirstDMIPlusCheck, Math.Round(dmiPlus.Value, 2));

            foreach (var val in valuesToAdd)
            {
                dmiPlus = function.AddNewValue(val);
            }

            Assert.NotNull(dmiPlus);
            Assert.AreEqual(valueFinalDMIPlusToCheck, Math.Round(dmiPlus.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunction()
        {
            var function = new DirectionalMovemnetIndexMinusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameCorrectData);

            var dmiMinusValue = function.WarmUp(values);

            double valueToCheck = 29.47;

            Assert.NotNull(dmiMinusValue);
            Assert.AreEqual(valueToCheck, Math.Round(dmiMinusValue.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunctionAddNew()
        {
            var function = new DirectionalMovemnetIndexMinusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameCorrectData);

            double? dmiMinusValue = 0.0d;

            foreach (var val in values)
            {
                dmiMinusValue = function.AddNewValue(val);
            }

            double valueToCheck = 29.47;

            Assert.NotNull(dmiMinusValue);
            Assert.AreEqual(valueToCheck, Math.Round(dmiMinusValue.Value, 2));
        }

        [Test]
        public void CalculateDMIMinusFunctionReturnNull()
        {
            var function = new DirectionalMovemnetIndexMinusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles(_fileNameNotFullData);

            double? dmiMinus = 0.0d;

            dmiMinus = function.WarmUp(values);
            Assert.IsNull(dmiMinus);
        }

        [Test]
        public void CalculateDMIMinusFunctionWarmUpAndAddTheRest()
        {
            var function = new DirectionalMovemnetIndexMinusFunction(new DMIParameters() { Priod = DEFAULT_PERCISION });
            var values = GetTestCandles("Adx_Examples2.txt").ToList();

            var valuesToWarmUp = values.GetRange(0, 15);
            var valuesToAdd = values.Skip(15).ToList();

            double? dmiMinus = 0.0d;
            double valueFirstDMIMinusCheck = 17.13;
            double valueFinalDMIMinusToCheck = 29.47;

            dmiMinus = function.WarmUp(valuesToWarmUp);

            Assert.NotNull(dmiMinus);
            Assert.AreEqual(valueFirstDMIMinusCheck, Math.Round(dmiMinus.Value, 2));

            foreach (var val in valuesToAdd)
            {
                dmiMinus = function.AddNewValue(val);
            }

            Assert.NotNull(dmiMinus);
            Assert.AreEqual(valueFinalDMIMinusToCheck, Math.Round(dmiMinus.Value, 2));
        }
    }
}
