using System;
using System.Collections.Generic;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class StatisticsRepositoryTests
    {
        private Statistics _entity;
        private static bool _entitySaved;
        private List<Statistics> _entitiesToSell;
        private static bool _entitiesToSellSaved;
        private List<Statistics> _entitiesToBuy;
        private static bool _entitiesToBuySaved;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //    var instanceId = SettingsMock.GetInstanceId();
        //    var repo = Given_Statistics_Repository();

        //    if (_entitySaved)
        //    {
        //        repo.DeleteAsync(_entity.InstanceId, AlgoInstanceType.Test, _entity.Id).Wait();
        //        _entitySaved = false;
        //    }

        //    _entity = null;

        //    if (_entitiesToBuySaved || _entitiesToSellSaved)
        //        repo.DeleteAllAsync(instanceId, AlgoInstanceType.Test).Wait(); //This will test deletion by partition key ;)
        //}

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateBuy_Test()
        {
            _entity = new Statistics
            {
                InstanceId = SettingsMock.GetInstanceId(),
                Amount = 123.45,
                Id = Guid.NewGuid().ToString(),
                IsBuy = true,
                Price = 123.45,
                InstanceType = AlgoInstanceType.Test
            };

            var repo = Given_Statistics_Repository();
            When_Invoke_CreateSingleEntity(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateSell_Test()
        {
            _entity = new Statistics
            {
                InstanceId = SettingsMock.GetInstanceId(),
                Amount = 123.45,
                Id = Guid.NewGuid().ToString(),
                IsBuy = false,
                Price = 123.45,
                InstanceType = AlgoInstanceType.Test
            };

            var repo = Given_Statistics_Repository();
            When_Invoke_CreateSingleEntity(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateMultipleBuy_GetStatistics_Test()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var repo = Given_Statistics_Repository();

            _entitiesToBuy = new List<Statistics>
            {
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = true,
                    Price = 1,
                    Amount = 1,
                    InstanceType = AlgoInstanceType.Test
                },
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = true,
                    Price = 2,
                    Amount = 2,
                    InstanceType = AlgoInstanceType.Test
                },
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = true,
                    Price = 3,
                    Amount = 3,
                    InstanceType = AlgoInstanceType.Test
                }
            };

            When_Invoke_CreateMultipleBuyEntities(repo);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_CreateMultipleSell_GetStatistics_Test()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var repo = Given_Statistics_Repository();

            _entitiesToSell = new List<Statistics>
            {
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = false,
                    Price = 1,
                    Amount = 1,
                    InstanceType = AlgoInstanceType.Test
                },
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = false,
                    Price = 2,
                    Amount = 2,
                    InstanceType = AlgoInstanceType.Test
                },
                new Statistics
                {
                    InstanceId = instanceId,
                    Id = Guid.NewGuid().ToString(),
                    IsBuy = false,
                    Price = 3,
                    Amount = 3,
                    InstanceType = AlgoInstanceType.Test
                }
            };

            When_Invoke_CreateMultipleSellEntities(repo);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void Statistics_AlgoStart_Test()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var repo = Given_Statistics_Repository();

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Id = Guid.NewGuid().ToString(),
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            When_Invoke_CreateSingleEntity(repo, _entity);
        }

        private void When_Invoke_CreateMultipleBuyEntities(StatisticsRepository repository)
        {
            if (_entitiesToBuy?.Count > 0)
            {
                foreach (var entity in _entitiesToBuy)
                {
                    repository.CreateAsync(entity).Wait();
                }
                _entitiesToBuySaved = true;
            }
        }

        private void When_Invoke_CreateMultipleSellEntities(StatisticsRepository repository)
        {
            if (_entitiesToSell?.Count > 0)
            {
                foreach (var entity in _entitiesToSell)
                {
                    repository.CreateAsync(entity).Wait();
                }
                _entitiesToSellSaved = true;
            }
        }

        private static void Then_Data_ShouldBe_Saved(StatisticsRepository repository, Statistics entity)
        {
            Assert.NotNull(entity);
        }

        private static void When_Invoke_CreateSingleEntity(StatisticsRepository repository, Statistics entity)
        {
            repository.CreateAsync(entity).Wait();
            _entitySaved = true;
        }

        private static StatisticsRepository Given_Statistics_Repository()
        {
            return new StatisticsRepository(
                AzureTableStorage<StatisticsEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()),
                AzureTableStorage<StatisticsSummaryEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()));
        }
    }
}
