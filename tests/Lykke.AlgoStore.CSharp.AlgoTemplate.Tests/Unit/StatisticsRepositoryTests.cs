using System;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entitites;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class StatisticsRepositoryTests
    {
        private Statistics _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_Statistics_Repository();

            if (_entitySaved)
            {
                repo.DeleteAsync(_entity.InstanceId, _entity.Id).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateBuy_Test()
        {
            _entity = new Statistics
            {
                InstanceId = SettingsMock.GetInstanceId().CurrentValue,
                Amount = 123.45,
                Id = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                IsBought = true,
                Price = 123.45
            };

            var repo = Given_Statistics_Repository();
            When_Invoke_Create(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateSell_Test()
        {
            _entity = new Statistics
            {
                InstanceId = SettingsMock.GetInstanceId().CurrentValue,
                Amount = 123.45,
                Id = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                IsBought = false,
                Price = 123.45
            };

            var repo = Given_Statistics_Repository();
            When_Invoke_Create(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        private static void Then_Data_ShouldBe_Saved(StatisticsRepository repository, Statistics entity)
        {
            Assert.NotNull(entity);
        }

        private static void When_Invoke_Create(StatisticsRepository repository, Statistics entity)
        {
            repository.CreateAsync(entity).Wait();
            _entitySaved = true;
        }

        private static StatisticsRepository Given_Statistics_Repository()
        {
            return new StatisticsRepository(AzureTableStorage<StatisticsEntity>.Create(
                SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()));
        }
    }
}
