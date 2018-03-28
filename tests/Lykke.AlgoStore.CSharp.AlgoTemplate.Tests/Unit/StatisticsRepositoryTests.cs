﻿using System;
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
        private StatisticsSummary _entitySummary;
        
        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [TearDown]
        public void CleanUp()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            repo.DeleteAllAsync(instanceId).Wait(); //This will test deletion by partition key ;)
        }

        [Test]
        public void BuyWithoutInitialSummaryThrowsException()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 123.45,
                IsBuy = true,
                Price = 123.45,
                InstanceType = instanceType
            };

            Assert.Throws<AggregateException>(() => WhenInvokeCreateEntity(repo, _entity));
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void InitializeSummaryWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.AssetOneBalance, summary.AssetOneBalance);
            Assert.AreEqual(_entitySummary.AssetTwoBalance, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
            Assert.AreEqual(0, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.AssetOneBalance, summary.AssetOneBalance);
            Assert.AreEqual(_entitySummary.AssetTwoBalance, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
            Assert.AreEqual(0, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedMultipleTimesWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 2, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.AssetOneBalance, summary.AssetOneBalance);
            Assert.AreEqual(_entitySummary.AssetTwoBalance, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
            Assert.AreEqual(0, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedWithOneBuyWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 1, summary.TotalNumberOfTrades);
            Assert.AreEqual(12, summary.AssetOneBalance);
            Assert.AreEqual(6, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(18, summary.LastWalletBalance);
            Assert.AreEqual((20d - 18d) / 20d, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedWithOneSellWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 1, summary.TotalNumberOfTrades);
            Assert.AreEqual(8, summary.AssetOneBalance);
            Assert.AreEqual(14, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(22, summary.LastWalletBalance);
            Assert.AreEqual((20d - 22d) / 20d, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedWithMultipleBuysWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 2, summary.TotalNumberOfTrades);
            Assert.AreEqual(14, summary.AssetOneBalance);
            Assert.AreEqual(2, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(16, summary.LastWalletBalance);
            Assert.AreEqual((20d - 16d) / 20d, summary.NetProfit);
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoStartedWithMultipleSellsWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var instanceType = AlgoInstanceType.Test;

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true,
                InstanceType = AlgoInstanceType.Test
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 2, summary.TotalNumberOfTrades);
            Assert.AreEqual(6, summary.AssetOneBalance);
            Assert.AreEqual(18, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(24, summary.LastWalletBalance);
            Assert.AreEqual((20d - 24d) / 20d, summary.NetProfit);
        }

        private static void WhenInvokeCreateEntity(StatisticsRepository repository, Statistics entity)
        {
            repository.CreateAsync(entity).Wait();
        }

        private static StatisticsSummary WhenInvokeGetSummary(
            StatisticsRepository repository,
            string instanceId,
            AlgoInstanceType instanceType)
        {
            return repository.GetSummaryAsync(instanceId).Result;
        }

        private static void WhenInvokeCreateSummaryEntity(StatisticsRepository repository, StatisticsSummary entitySummary)
        {
            repository.CreateOrUpdateSummaryAsync(entitySummary).Wait();
        }

        private static StatisticsRepository GivenStatisticsRepository()
        {
            return new StatisticsRepository(
                AzureTableStorage<StatisticsEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()),
                AzureTableStorage<StatisticsSummaryEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()));
        }
    }
}
