using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class RabbitMqQuoteProviderServiceTests
    {
        private string _assetPair = "BTCUSD";

        private IAlgoSettingsService AlgoSettingsService()
        {
            var algoSettings = new Mock<IAlgoSettingsService>();
            algoSettings.Setup(a => a.GetInstanceId()).Returns(Guid.NewGuid().ToString());
            return algoSettings.Object;
        }

        [Test, Explicit("Test real connection to RabbitMq")]
        public void RabbitMq_Success_Test()
        {
            var algoSettings = AlgoSettingsService();
            var settings = SettingsMock.GetQuoteSettings();
            Assert.IsNotNull(settings);

            IQuoteProviderService quoteProviderService = new RabbitMqQuoteProviderService(settings, new LogMock(), algoSettings);
            quoteProviderService.Initialize().Wait();

            quoteProviderService.Subscribe(_assetPair, CorrectSubscriber);

            quoteProviderService.Subscribe(_assetPair, ExceptionSubscriber);

            quoteProviderService.Start();

            Task.Delay(100000).Wait();

            quoteProviderService.Stop();
        }

        private async Task CorrectSubscriber(IAlgoQuote quote)
        {
            Assert.IsNotNull(quote);
            Debug.WriteLine("========================= QUOTE ===" + quote.Price);
            await Task.Delay(1000);
        }

        private Task ExceptionSubscriber(IAlgoQuote quote)
        {
            Assert.IsNotNull(quote);
            Debug.WriteLine("==================== EXCEPTION QUOTE ===" + quote.Price);
            throw new Exception("ExceptionSubscriber");
        }
    }
}
