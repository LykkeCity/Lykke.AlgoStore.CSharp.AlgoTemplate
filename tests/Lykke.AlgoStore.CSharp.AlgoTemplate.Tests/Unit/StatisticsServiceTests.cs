using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
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
        }

        [Test]
        public void Create_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            When_Invoke_Create(service, out var exception);

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

        private static void Then_Exception_ShouldNotBe_Null(Exception exception)
        {
            Assert.NotNull(exception);
        }

        private static void Then_Data_ShouldNotBe_Zero(double data)
        {
            Assert.Greater(data, 0);
        }

        private static void Then_Data_ShouldBe_Zero(double data)
        {
            Assert.AreEqual(data, 0);
        }

        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }

        private static IStatisticsService Given_StatisticsService(IStatisticsRepository repo)
        {
            var statisticsService = new StatisticsService(repo, SettingsMock.InitSettingsService());

            statisticsService.OnAlgoStarted();

            return statisticsService;
        }

        private IStatisticsRepository Given_Correct_StatisticsRepositoryMock()
        {
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.CreateAsync(new Statistics()));

            return result.Object;
        }

        private IStatisticsRepository Given_Error_StatisticsRepositoryMock()
        {
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.CreateAsync(It.IsAny<Statistics>())).ThrowsAsync(new Exception("CreateAsync"));

            return result.Object;
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
    }
}
