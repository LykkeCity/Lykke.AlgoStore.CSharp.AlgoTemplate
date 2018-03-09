using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class CandlesServiceTests
    {
        [Test]
        public void StartProducing_ThrowsNotSupported_WhenNoSubscriptions()
        {
            var candlesService = new CandlesService(null, null);

            Assert.Throws<NotSupportedException>(() => candlesService.StartProducing());
        }

        [Test]
        public void StartProducing_ThrowsInvalidOperation_WhenAlreadyProducing()
        {
            var providerMock = CreateProviderMock();

            var requests = new List<CandleServiceRequest>();
            Action<IList<MultipleCandlesResponse>> initialDataConsumer = (data) => { };

            var candlesService = new CandlesService(providerMock.Object, null);
            candlesService.Subscribe(requests, initialDataConsumer, null);

            candlesService.StartProducing();

            Assert.Throws<InvalidOperationException>(() => candlesService.StartProducing());
        }

        [Test]
        public void StartProducing_FillsCandlesCorrectly_WhenHavingEmptyRange()
        {
            var providerMock = CreateProviderMock();

            var requests = new List<CandleServiceRequest>
            {
                new CandleServiceRequest
                {
                    CandleInterval = CandleTimeInterval.Day,
                    RequestId = "a",
                }
            };

            var firstCandle = new Candle
            {
                Open = 1,
                Close = 2,
                High = 3,
                Low = 0,
                DateTime = DateTime.Now.AddDays(-2).AddHours(-12)
            };

            var historyMock = new Mock<IHistoryDataService>();
            historyMock.Setup(h => h.GetHistory(It.IsAny<CandlesHistoryRequest>()))
                       .Returns(new List<Candle> { firstCandle });

            // The result verifier
            Action<IList<MultipleCandlesResponse>> initialDataConsumer = (data) => 
            {
                Assert.AreEqual(1, data.Count);

                var response = data[0];

                Assert.AreEqual("a", response.RequestId);

                var candles = response.Candles;
                Assert.AreEqual(2, candles.Count);

                Assert.AreEqual(firstCandle, candles[0]);

                Assert.AreEqual(2, candles[1].Open);
                Assert.AreEqual(2, candles[1].Close);
                Assert.AreEqual(2, candles[1].High);
                Assert.AreEqual(2, candles[1].Low);
                Assert.AreEqual(firstCandle.DateTime.AddDays(1), candles[1].DateTime);
            };

            var candlesService = new CandlesService(providerMock.Object, historyMock.Object);
            candlesService.Subscribe(requests, initialDataConsumer, null);

            candlesService.StartProducing();
        }

        [Test]
        public void Subscribe_ThrowsNotSupported_WhenInvokedMoreThanOnce()
        {
            var candlesService = new CandlesService(null, null);
            var requests = new List<CandleServiceRequest>();
            Action<IList<MultipleCandlesResponse>> initialDataConsumer = (data) => { };

            candlesService.Subscribe(requests, initialDataConsumer, null);

            Assert.Throws<NotSupportedException>(() => candlesService.Subscribe(requests, initialDataConsumer, null));
        }

        private Mock<ICandleProviderService> CreateProviderMock()
        {
            var providerMock = new Mock<ICandleProviderService>();
            providerMock.Setup(p => p.Initialize())
                        .Returns(Task.CompletedTask);

            return providerMock;
        }
    }
}
