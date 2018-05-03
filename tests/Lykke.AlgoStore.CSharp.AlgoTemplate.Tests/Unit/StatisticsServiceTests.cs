using System;
using System.Threading.Tasks;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Moq;
using NUnit.Framework;
using Mapper = AutoMapper.Mapper;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class StatisticsServiceTests
    {
        private string _instanceId;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();

            _instanceId = SettingsMock.GetInstanceId();
            SettingsMock.GetInstanceType();
        }

        [Test]
        public void Create_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);

            When_Invoke_OnStart(service, out var exception);
            Then_Exception_ShouldBe_Null(exception);

            When_Invoke_Create(service, out exception);
            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void Create_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_Create(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
        }

        [Test]
        public void GetSummary_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_GetSummary(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void GetSummary_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_GetSummary(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
        }

        [Test]
        public void OnStart_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_OnStart(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void OnStart_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_OnStart(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
        }

        private static void When_Invoke_OnStart(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                service.OnAlgoStarted();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        private static void Then_Exception_ShouldNotBe_Null(Exception exception)
        {
            Assert.NotNull(exception);
        }

        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }

        private static IStatisticsService Given_StatisticsService(IStatisticsRepository repo)
        {
            var statisticsService = new StatisticsService(repo, SettingsMock.InitSettingsService());

            return statisticsService;
        }

        private IStatisticsRepository Given_Correct_StatisticsRepositoryMock()
        {
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSummaryAsync(_instanceId)).Returns(Task.FromResult(new StatisticsSummary()));

            return result.Object;
        }

        private static IStatisticsRepository Given_Error_StatisticsRepositoryMock()
        {
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSummaryAsync(It.IsAny<string>())).ThrowsAsync(new Exception("GetSummary"));

            return result.Object;
        }

        private static UserLogRepository Given_Correct_UserLog_Repository()
        {
            return new UserLogRepository(
                AzureTableStorage<UserLogEntity>.Create(SettingsMock.GetLogsConnectionString(), UserLogRepository.TableName, new LogMock())
            );
        }

        private static void When_Invoke_Create(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                service.OnAction(true, 1, 1);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        private static void When_Invoke_GetSummary(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                service.GetSummary();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
    }
}
