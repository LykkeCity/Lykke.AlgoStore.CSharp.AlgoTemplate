using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class SmaFunctionTests
    {
        private static double[] CustomPriceValues => Utils.GenerateRandomDoubles(100).ToArray();

        [Test]
        public void CalculateSmaTest()
        {
            var smaFunction = new SmaFunction
            {
                Parameters = new SmaParameters
                {
                    LongTermPeriod = 50,
                    ShortTermPeriod = 20
                },
                Values = CustomPriceValues
            };

            smaFunction.CalculateSma();

            Assert.IsNotEmpty(smaFunction.SmaLongTerm);
            Assert.IsNotEmpty(smaFunction.SmaShortTerm);
        }
    }

}
