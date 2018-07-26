using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class StatisticsRepositoryTests
    {
        private StatisticsSummary _entitySummary;

        private readonly string _instanceId = "18169b36-a51c-4f8c-8d17-09a45f0f41m6";
        private readonly string _rowKey = "Summary";

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();
        }


        //REMARK: Please uncomment test below if you need to get summary for REAl algo instance
        //[Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        //public void GetAlgoInstanceSummaryForRealAlgoInstance()
        //{
        //    var instanceId = "1d34526b-814d-4f8c-83de-28b2c03c7aca";
        //    var repo = GivenStatisticsRepository();

        //    var statistics = WhenInvokeGetSummary(repo, instanceId);

        //    Assert.AreEqual(1, statistics.TotalNumberOfStarts);
        //}

        //    //REMARK: Please uncomment test below if you need to create summary row for specific instance (when you want to test something specific)
        //    //Be aware that this test wont mop up at the end so new row will stay in storage
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
        //        InitialTradedAssetBalance = 5000,
        //        InitialAssetTwoBalance = 5000,
        //        LastTradedAssetBalance = 5000,
        //        LastAssetTwoBalance = 5000,
        //    };

        //    WhenInvokeCreateSummaryEntity(repo, _entitySummary);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.InitialTradedAssetBalance, summary.InitialTradedAssetBalance);
            Assert.AreEqual(_entitySummary.InitialAssetTwoBalance, summary.InitialAssetTwoBalance);
            Assert.AreEqual(_entitySummary.LastTradedAssetBalance, summary.LastTradedAssetBalance);
            Assert.AreEqual(_entitySummary.LastAssetTwoBalance, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.InitialTradedAssetBalance, summary.InitialTradedAssetBalance);
            Assert.AreEqual(_entitySummary.InitialAssetTwoBalance, summary.InitialAssetTwoBalance);
            Assert.AreEqual(_entitySummary.LastTradedAssetBalance, summary.LastTradedAssetBalance);
            Assert.AreEqual(_entitySummary.LastAssetTwoBalance, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(_entitySummary.InitialTradedAssetBalance, summary.InitialTradedAssetBalance);
            Assert.AreEqual(_entitySummary.InitialAssetTwoBalance, summary.InitialAssetTwoBalance);
            Assert.AreEqual(_entitySummary.LastTradedAssetBalance, summary.LastTradedAssetBalance);
            Assert.AreEqual(_entitySummary.LastAssetTwoBalance, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(_entitySummary.LastWalletBalance, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 12;
            _entitySummary.LastAssetTwoBalance = 6;
            _entitySummary.LastWalletBalance = 18;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(12, summary.LastTradedAssetBalance);
            Assert.AreEqual(6, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(18, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 8;
            _entitySummary.LastAssetTwoBalance = 14;
            _entitySummary.LastWalletBalance = 22;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(8, summary.LastTradedAssetBalance);
            Assert.AreEqual(14, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(22, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 12;
            _entitySummary.LastAssetTwoBalance = 6;
            _entitySummary.LastWalletBalance = 18;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 14;
            _entitySummary.LastAssetTwoBalance = 2;
            _entitySummary.LastWalletBalance = 16;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(14, summary.LastTradedAssetBalance);
            Assert.AreEqual(2, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(16, summary.LastWalletBalance);
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
                InitialTradedAssetBalance = 10,
                InitialAssetTwoBalance = 10,
                LastTradedAssetBalance = 10,
                LastAssetTwoBalance = 10,
                UserCurrencyBaseAssetId = "USD"
            };

            WhenInvokeCreateSummaryEntity(repo, _entitySummary);

            _entitySummary.TotalNumberOfStarts++;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 8;
            _entitySummary.LastAssetTwoBalance = 14;
            _entitySummary.LastWalletBalance = 22;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            _entitySummary.TotalNumberOfTrades++;
            _entitySummary.LastTradedAssetBalance = 6;
            _entitySummary.LastAssetTwoBalance = 18;
            _entitySummary.LastWalletBalance = 24;

            WhenInvokeCreateStatisticsEntityWithSummary(repo, _entitySummary);

            var summary = WhenInvokeGetSummary(repo, instanceId);

            Assert.AreEqual(_entitySummary.InstanceId, summary.InstanceId);
            Assert.AreEqual(_entitySummary.TotalNumberOfStarts, summary.TotalNumberOfStarts);
            Assert.AreEqual(_entitySummary.TotalNumberOfTrades, summary.TotalNumberOfTrades);
            Assert.AreEqual(6, summary.LastTradedAssetBalance);
            Assert.AreEqual(18, summary.LastAssetTwoBalance);
            Assert.AreEqual(_entitySummary.InitialWalletBalance, summary.InitialWalletBalance);
            Assert.AreEqual(24, summary.LastWalletBalance);
        }


        [Test]
        public void StatisticsSummary_TestDeleteSummaryAsync()
        {
            var storage = new Mock<INoSQLTableStorage<StatisticsSummaryEntity>>();

            storage.Setup(s => s.DeleteIfExistAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string partionKey, string rowKey) =>
                {
                    CheckIfInsatanceIsTheSame(partionKey, rowKey);
                    return Task.FromResult(true);
                });

            storage.Setup(s => s.DeleteIfExistAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string partionKey, string rowKey) =>
                {
                    CheckIfInsatanceIsTheSame(partionKey, rowKey);
                    return Task.FromResult(false);
                });

            StatisticsRepository repository = new StatisticsRepository(storage.Object);
            repository.DeleteSummaryAsync(_instanceId).Wait();
        }

        private void CheckIfInsatanceIsTheSame(string instanceIdToCheck, string rowKeyToCheck)
        {
            Assert.AreEqual(_instanceId, instanceIdToCheck);
            Assert.AreEqual(_rowKey, rowKeyToCheck);
        }

        private static void WhenInvokeCreateStatisticsEntityWithSummary(
            StatisticsRepository repository,
            StatisticsSummary summary)
        {
            repository.CreateAsync(summary).Wait();
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
            return new StatisticsRepository(AzureTableStorage<StatisticsSummaryEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), StatisticsRepository.TableName, new LogMock()));
        }

    }
}
