using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Moq;
using NUnit.Framework;
using Remotion.Linq.Utilities;

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

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            _instanceId = SettingsMock.GetInstanceId().CurrentValue;
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

        [Test]
        public void GetBoughtAmount_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetBoughtAmount(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Zero(data);
        }

        [Test]
        public void GetBoughtAmount_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetBoughtAmount(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
            Then_Data_ShouldBe_Zero(data);
        }

        [Test]
        public void GetSoldAmount_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetSoldAmount(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Zero(data);
        }

        [Test]
        public void GetSoldAmount_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetSoldAmount(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
            Then_Data_ShouldBe_Zero(data);
        }

        [Test]
        public void GetBoughtQuantity_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetBoughtQuantity(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Zero(data);
        }

        [Test]
        public void GetBoughtQuantity_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetBoughtQuantity(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
            Then_Data_ShouldBe_Zero(data);
        }

        [Test]
        public void GetSoldQuantity_Returns_Data()
        {
            var repo = Given_Correct_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetSoldQuantity(service, out var exception);

            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Zero(data);
        }

        [Test]
        public void GetSoldQuantity_Throws_Exception()
        {
            var repo = Given_Error_StatisticsRepositoryMock();
            var service = Given_StatisticsService(repo);
            var data = When_Invoke_GetSoldQuantity(service, out var exception);

            Then_Exception_ShouldNotBe_Null(exception);
            Then_Data_ShouldBe_Zero(data);
        }

        private static void Then_Exception_ShouldNotBe_Null(Exception exception)
        {
            Assert.NotNull(exception);
        }

        private static double When_Invoke_GetSoldQuantity(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetSoldQuantity();
            }
            catch (Exception ex)
            {
                exception = ex;
                return 0;
            }
        }

        private static double When_Invoke_GetBoughtQuantity(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetBoughtQuantity();
            }
            catch (Exception ex)
            {
                exception = ex;
                return 0;
            }
        }

        private static double When_Invoke_GetSoldAmount(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetSoldAmount();
            }
            catch (Exception ex)
            {
                exception = ex;
                return 0;
            }
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

        private static double When_Invoke_GetBoughtAmount(IStatisticsService service, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetBoughtAmount();
            }
            catch (Exception ex)
            {
                exception = ex;
                return 0;
            }
        }

        private IStatisticsService Given_StatisticsService(IStatisticsRepository repo)
        {
            return new StatisticsService(repo, _instanceId);
        }

        private IStatisticsRepository Given_Correct_StatisticsRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSoldQuantityAsync(_instanceId)).Returns(() => Task.FromResult<double>(5));
            result.Setup(repo => repo.GetBoughtAmountAsync(_instanceId)).Returns(() => Task.FromResult<double>(5));
            result.Setup(repo => repo.GetBoughtQuantityAsync(_instanceId)).Returns(() => Task.FromResult<double>(5));
            result.Setup(repo => repo.GetSoldAmountAsync(_instanceId)).Returns(() => Task.FromResult<double>(5));
            result.Setup(repo => repo.CreateAsync(new Statistics()));

            return result.Object;
        }

        private IStatisticsRepository Given_Error_StatisticsRepositoryMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IStatisticsRepository>();

            result.Setup(repo => repo.GetSoldQuantityAsync(_instanceId)).ThrowsAsync(new Exception("GetSoldQuantity"));
            result.Setup(repo => repo.GetBoughtAmountAsync(_instanceId)).ThrowsAsync(new Exception("GetBoughtAmount"));
            result.Setup(repo => repo.GetBoughtQuantityAsync(_instanceId)).ThrowsAsync(new Exception("GetBoughtQuantity"));
            result.Setup(repo => repo.GetSoldAmountAsync(_instanceId)).ThrowsAsync(new Exception("GetSoldAmount"));
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
