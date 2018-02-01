using System;
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
