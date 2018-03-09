using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using NUnit.Framework;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class CandleTimeIntervalExtensionsTests
    {
        [Test]
        public void IncrementTimestamp_CalculatesCorrectly_ForMonth()
        {
            var timestamp = new DateTime(2018, 1, 1);

            var result = CandleTimeInterval.Month.IncrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 2, 1), result);
        }

        [Test]
        public void IncrementTimestamp_CalculatesCorrectly_ForHour4()
        {
            var timestamp = new DateTime(2018, 1, 1, 0, 0, 0);

            var result = CandleTimeInterval.Hour4.IncrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 1, 1, 4, 0, 0), result);
        }

        [Test]
        public void IncrementTimestamp_CalculatesCorrectly_ForOthers()
        {
            var timestamp = new DateTime(2018, 1, 1);

            var result = CandleTimeInterval.Day.IncrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 1, 2), result);
        }

        [Test]
        public void DecrementTimestamp_CalculatesCorrectly_ForMonth()
        {
            var timestamp = new DateTime(2018, 2, 1);

            var result = CandleTimeInterval.Month.DecrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 1, 1), result);
        }

        [Test]
        public void DecrementTimestamp_CalculatesCorrectly_ForHour4()
        {
            var timestamp = new DateTime(2018, 1, 1, 4, 0, 0);

            var result = CandleTimeInterval.Hour4.DecrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 1, 1, 0, 0, 0), result);
        }

        [Test]
        public void DecrementTimestamp_CalculatesCorrectly_ForOthers()
        {
            var timestamp = new DateTime(2018, 1, 2);

            var result = CandleTimeInterval.Day.DecrementTimestamp(timestamp);

            Assert.AreEqual(new DateTime(2018, 1, 1), result);
        }
    }
}
