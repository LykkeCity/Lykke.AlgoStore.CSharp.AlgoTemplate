using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Services.Services
{
    [TestFixture]
    public class RandomDataQuoteProviderServiceTest
    {
        private string _assetPair = "BTCUSD";

        [Test]
        public void GenerateTick_InvokesSubscriber()
        {
            // Create mocks
            var actionMock = new Mock<Func<IAlgoQuote, Task>>();
            var asyncExecutorMock = new Mock<IAsyncExecutor>();
            asyncExecutorMock.Setup(e => e.ExecuteAsync(actionMock.Object, It.IsNotNull<IAlgoQuote>()));

            // Subscribe and call generate tick
            RandomDataQuoteProviderService quoteProvider = new RandomDataQuoteProviderService(asyncExecutorMock.Object);
            quoteProvider.Subscribe(_assetPair, actionMock.Object);
            quoteProvider.GenerateTick();

            // assert invocation
            asyncExecutorMock.Verify(e => e.ExecuteAsync(actionMock.Object, It.IsNotNull<IAlgoQuote>()), Times.Once);
        }

        [Test]
        public void GenerateTick_InvokesMultipleSubscribers()
        {
            // Create mocks
            var action1Mock = new Mock<Func<IAlgoQuote, Task>>();
            var action2Mock = new Mock<Func<IAlgoQuote, Task>>();
            var asyncExecutorMock = new Mock<IAsyncExecutor>();
            asyncExecutorMock.Setup(e => e.ExecuteAsync(action1Mock.Object, It.IsNotNull<IAlgoQuote>()));
            asyncExecutorMock.Setup(e => e.ExecuteAsync(action2Mock.Object, It.IsNotNull<IAlgoQuote>()));

            // Subscribe and call generate tick
            RandomDataQuoteProviderService quoteProvider = new RandomDataQuoteProviderService(asyncExecutorMock.Object);
            quoteProvider.Subscribe(_assetPair, action1Mock.Object);
            quoteProvider.Subscribe(_assetPair, action2Mock.Object);
            quoteProvider.GenerateTick();

            // assert invocation
            asyncExecutorMock.Verify(e => e.ExecuteAsync(action1Mock.Object, It.IsNotNull<IAlgoQuote>()), Times.Once);
            asyncExecutorMock.Verify(e => e.ExecuteAsync(action2Mock.Object, It.IsNotNull<IAlgoQuote>()), Times.Once);
        }

        [Test]
        public void GenerateTick_WithNoSubscriber()
        {
            RandomDataQuoteProviderService quoteProvider = new RandomDataQuoteProviderService();

            quoteProvider.GenerateTick();
        }
    }

}
