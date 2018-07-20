using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Job.Stopping.Client.Models.ResponseModels;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        private const string PLACEHOLDER_INSTANCE_ID = "instanceid";
        private const string PLACEHOLDER_AUTH_TOKEN = "authtoken";

        [Test]
        public async Task MonitoringService_StopsAlgo_WhenTimeExpires()
        {
            var settingsServiceMock = Given_Correct_VerifiableSettingsServiceMock();
            var stoppingClientMock = Given_Correct_VerifiableInstanceStoppingClient();
            var monitoringService = new MonitoringService(
                settingsServiceMock.Object,
                stoppingClientMock.Object,
                Mock.Of<IUserLogService>(),
                Mock.Of<ILog>(),
                TimeSpan.FromMilliseconds(500));

            monitoringService.StartAlgoEvent("");

            await Task.Delay(1000);

            stoppingClientMock.Verify();
            settingsServiceMock.Verify();
        }

        [Test]
        public async Task MonitoringService_DoesNotStopAlgo_WhenTokenCancelled()
        {
            var monitoringService = new MonitoringService(
                Mock.Of<IAlgoSettingsService>(),
                Given_Failing_InstanceStoppingClient(),
                Mock.Of<IUserLogService>(),
                Mock.Of<ILog>(),
                TimeSpan.FromMilliseconds(500));

            monitoringService.StartAlgoEvent("").Cancel();

            await Task.Delay(1000);
        }

        private Mock<IAlgoSettingsService> Given_Correct_VerifiableSettingsServiceMock()
        {
            var mock = new Mock<IAlgoSettingsService>(MockBehavior.Strict);

            mock.Setup(s => s.GetInstanceId())
                .Returns(PLACEHOLDER_INSTANCE_ID)
                .Verifiable();

            mock.Setup(s => s.GetAuthToken())
                .Returns(PLACEHOLDER_AUTH_TOKEN)
                .Verifiable();

            return mock;
        }

        private Mock<IAlgoInstanceStoppingClient> Given_Correct_VerifiableInstanceStoppingClient()
        {
            var mock = new Mock<IAlgoInstanceStoppingClient>(MockBehavior.Strict);

            mock.Setup(c => c.DeleteAlgoInstanceAsync(PLACEHOLDER_INSTANCE_ID, PLACEHOLDER_AUTH_TOKEN))
                .ReturnsAsync(new DeleteAlgoInstanceResponseModel
                {
                    IsSuccessfulDeletion = true
                })
                .Verifiable();

            return mock;
        }

        private IAlgoInstanceStoppingClient Given_Failing_InstanceStoppingClient()
        {
            var mock = new Mock<IAlgoInstanceStoppingClient>(MockBehavior.Strict);

            mock.Setup(c => c.DeleteAlgoInstanceAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback(() => Assert.Fail());

            return mock.Object;
        }
    }
}
