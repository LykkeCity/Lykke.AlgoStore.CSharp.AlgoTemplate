﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
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

            var repo = GivenStatisticsRepository();

            repo.DeleteAllAsync(instanceId).Wait(); //This will test deletion by partition key ;)
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void GetAllAlgoInstanceStatistics()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var repo = GivenStatisticsRepository();

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            WhenInvokeCreateEntity(repo, _entity);

            var statistics = WhenInvokeGetStatistics(repo, instanceId);

            Assert.AreEqual(1, statistics.Count);
        }

        //REMARK: Please uncomment test below if you need to create summary row for specific instance (when you want to test something specific)
        //Be aware that this test wont mop up at the end so new row will stay in storage
        //[Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        //public void CreateSummaryRow()
        //{
        //    //REMARK: Use only when you need to insert new summary row for some reason :)
        //    var instanceId = "MJ_TEST_1234567890";

        //    var repo = GivenStatisticsRepository();

        //    _entitySummary = new StatisticsSummary
        //    {
        //        InstanceId = instanceId,
        //        TotalNumberOfTrades = 0,
        //        TotalNumberOfStarts = 0,
        //        InitialWalletBalance = 10000,
        //        LastWalletBalance = 10000,
        //        AssetTwoBalance = 5000,
        //        AssetOneBalance = 5000
        //    };

        //    WhenInvokeCreateSummaryEntity(repo, _entitySummary);
        //}

        //REMARK: Please uncomment test below if you need to create multiple rows for specific instance (when you want to test something specific)
        //Be aware that there is a line at the end that should mop up at the end
        //[Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        //public void CreateMoreThen1000Rows()
        //{
        //    var instanceId = "MJ_TEST_1234567890";
        //    var repo = GivenStatisticsRepository();
        //    var numberOfEntities = 100;
        //    var numberOfEntitiesToFetch = 100;
        //    var random = new Random();

        //    for (int i = 0; i < numberOfEntities; i++)
        //    {
        //        _entity = new Statistics
        //        {
        //            InstanceId = instanceId,
        //            Amount = random.NextDouble() * (10 - 1) + 1,
        //            IsBuy = random.NextDouble() >= 0.5,
        //            Price = random.NextDouble() * (10 - 1) + 1
        //        };

        //        WhenInvokeCreateEntity(repo, _entity);
        //    }

        //    var timer = new Stopwatch();
        //    timer.Start();

        //    var allEntities = repo.GetAllStatisticsAsync(instanceId).Result;

        //    timer.Stop();

        //    Assert.AreEqual(numberOfEntities, allEntities.Count);
        //    Assert.Warn($"All entities fetched in {timer.ElapsedMilliseconds}ms");

        //    timer.Start();

        //    allEntities = repo.GetAllStatisticsAsync(instanceId, numberOfEntitiesToFetch).Result;

        //    timer.Stop();

        //    Assert.AreEqual(numberOfEntitiesToFetch, allEntities.Count);
        //    Assert.Warn($"{numberOfEntitiesToFetch} entities fetched in {timer.ElapsedMilliseconds}ms");

        //    repo.DeleteAllAsync(instanceId).Wait();
        //}

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void InitializeSummaryWillReturnValidSummary()
        {
            var instanceId = SettingsMock.GetInstanceId();

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
                TotalNumberOfTrades = 0,
                TotalNumberOfStarts = 0,
                InitialWalletBalance = 20,
                LastWalletBalance = 20,
                AssetTwoBalance = 10,
                AssetOneBalance = 10
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 12;
            _entitySummary.AssetTwoBalance = 6;
            _entitySummary.LastWalletBalance = 18;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 8;
            _entitySummary.AssetTwoBalance = 14;
            _entitySummary.LastWalletBalance = 22;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 12;
            _entitySummary.AssetTwoBalance = 6;
            _entitySummary.LastWalletBalance = 18;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 14;
            _entitySummary.AssetTwoBalance = 2;
            _entitySummary.LastWalletBalance = 16;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
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

            var repo = GivenStatisticsRepository();

            _entitySummary = new StatisticsSummary
            {
                InstanceId = instanceId,
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
                IsStarted = true
            };

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 8;
            _entitySummary.AssetTwoBalance = 14;
            _entitySummary.LastWalletBalance = 22;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            _entity = new Statistics
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = false,
                Price = 2
            };

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.AssetOneBalance = 6;
            _entitySummary.AssetTwoBalance = 18;
            _entitySummary.LastWalletBalance = 24;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entity, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(6, summary.AssetOneBalance);
            Assert.AreEqual(18, summary.AssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(24, summary.LastWalletBalance);
            Assert.AreEqual((20d - 24d) / 20d, summary.NetProfit);
        }

        private static List<Statistics> WhenInvokeGetStatistics(StatisticsRepository repository, string instanceId)
        {
            return repository.GetAllStatisticsAsync(instanceId).Result;
        }

        private static void WhenInvokeCreateStatisticsEntityWithSummary(
            StatisticsRepository repository, 
            Statistics entity,
            StatisticsSummary summary)
        {
            repository.CreateAsync(entity, summary).Wait();
        }

        private static void WhenInvokeCreateEntity(StatisticsRepository repository, Statistics entity)
        {
            repository.CreateAsync(entity).Wait();
        }

        private static StatisticsSummary WhenInvokeGetSummary(
            StatisticsRepository repository,
            string instanceId)
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
