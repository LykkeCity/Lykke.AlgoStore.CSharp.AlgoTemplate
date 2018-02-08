using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class RabbitMqQuoteProviderServiceTests
    {
        [Test, Explicit("Test real connection to RabbitMq")]
        public void RabbitMq_Success_Test()
        {
            var settings = SettingsMock.GetQuoteSettings();
            Assert.IsNotNull(settings);

            IQuoteProviderService quoteProviderService = new RabbitMqQuoteProviderService(settings, new LogMock());
            quoteProviderService.Initialize().Wait();

            quoteProviderService.Subscribe(CorrectSubscriber);

            quoteProviderService.Subscribe(ExceptionSubscriber);

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
