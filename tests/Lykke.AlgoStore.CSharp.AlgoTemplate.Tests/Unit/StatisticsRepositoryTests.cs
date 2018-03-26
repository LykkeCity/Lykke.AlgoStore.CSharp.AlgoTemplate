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

            repo.DeleteAllAsync(instanceId, instanceType).Wait(); //This will test deletion by partition key ;)

            instanceType = AlgoInstanceType.Demo;

            repo.DeleteAllAsync(instanceId, instanceType).Wait();
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
                InitialWalletBalance = 0,
                AssetTwoBalance = 0,
                AssetOneBalance = 0
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
                InitialWalletBalance = 0,
                AssetTwoBalance = 0,
                AssetOneBalance = 0
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
                InitialWalletBalance = 10,
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
                Amount = 1,
                IsBuy = true,
                Price = 1,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 1, summary.TotalNumberOfTrades);

            //REMARK: Change lines below as soon as new math is done
            //Assert.AreEqual(_entitySummary.AssetOneBalance, summary.AssetOneBalance);
            //Assert.AreEqual(_entitySummary.AssetTwoBalance, summary.AssetTwoBalance);
            //Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            //Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
            
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
                InitialWalletBalance = 10,
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
                Amount = 1,
                IsBuy = false,
                Price = 1,
                InstanceType = instanceType
            };

            WhenInvokeCreateEntity(repo, _entity);

            var summary = WhenInvokeGetSummary(repo, instanceId, instanceType);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.InstanceType, summary.InstanceType);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts + 1, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades + 1, summary.TotalNumberOfTrades);

            //REMARK: Change lines below as soon as new math is done
            //Assert.AreEqual(_entitySummary.AssetOneBalance, summary.AssetOneBalance);
            //Assert.AreEqual(_entitySummary.AssetTwoBalance, summary.AssetTwoBalance);
            //Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            //Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);

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
            return repository.GetSummary(instanceId, instanceType).Result;
        }

        private static void WhenInvokeCreateSummaryEntity(StatisticsRepository repository, StatisticsSummary entitySummary)
        {
            repository.CreateSummary(entitySummary).Wait();
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
