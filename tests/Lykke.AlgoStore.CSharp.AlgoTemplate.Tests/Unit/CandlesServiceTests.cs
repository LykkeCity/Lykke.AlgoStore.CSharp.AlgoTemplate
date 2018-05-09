using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
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
            var candlesService = new CandlesService(null, null, null);

            Assert.Throws<NotSupportedException>(() => candlesService.StartProducing());
        }

        [Test]
        public void StartProducing_ThrowsInvalidOperation_WhenAlreadyProducing()
        {
            var providerMock = CreateProviderMock();

            var requests = new List<CandleServiceRequest>();
            Action<IList<MultipleCandlesResponse>> initialDataConsumer = (data) => { };

            var candlesService = new CandlesService(providerMock.Object, null, Given_Correct_AlgoSettingsService());
            candlesService.Subscribe(requests, initialDataConsumer, null);

            candlesService.StartProducing();

            Assert.Throws<InvalidOperationException>(() => candlesService.StartProducing());
        }

        [Test]
        public void Subscribe_ThrowsNotSupported_WhenInvokedMoreThanOnce()
        {
            var candlesService = new CandlesService(null, null, null);
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

        private IAlgoSettingsService Given_Correct_AlgoSettingsService()
        {
            var mock = new Mock<IAlgoSettingsService>();

            return mock.Object;
        }
    }
}
