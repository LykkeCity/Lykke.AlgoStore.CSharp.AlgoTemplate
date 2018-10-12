using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.Common.Log;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class AlgoInstanceTradeRepositoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        private AlgoInstanceTrade _entity;

        private readonly string _instanceId = "17169b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _orderId = "11269b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _walletId = "66269b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _assetId = "USD";
        private readonly double _amount = 10;
        private readonly int _itemCount = 10;

        [SetUp]
        public void SetUp()
        {
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void AlgoTrades_GetAllAlgoInstanceTrade()
        {
            var repo = GivenAlgoInstanceTradeRepository();

            _entity = new AlgoInstanceTrade
            {
                InstanceId = _instanceId,
                AssetId = _assetId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            WhenInvokeCreateEntity(repo, _entity);

            var statistics = WhenInvokeGetTrades(repo, _instanceId, _assetId);
            Assert.AreEqual(1, statistics.ToList().Count);
        }

        [Test]
        public void AlgoTrades_TestGeneratePartitionKeyByInstanceIdAndAssetId()
        {
            string correctResult = _instanceId + "_" + _assetId;
            string resultToTest =
                AlgoInstanceTradeRepository.GeneratePartitionKeyByInstanceIdAndAssetId(_instanceId, _assetId);

            Assert.AreEqual(correctResult, resultToTest);
        }

        [Test]
        public void AlgoTrades_TestCreateAlgoInstanceOrderAsync()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            storage.Setup(s => s.InsertAsync(It.IsAny<AlgoInstanceTradeEntity>()))
                .Returns((AlgoInstanceTradeEntity entity, int[] parameters) =>
                {
                    CheckIfOrderEntityIsCorrect(entity);
                    return Task.CompletedTask;
                });

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            repository.CreateOrUpdateAlgoInstanceOrderAsync(new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                OrderId = _orderId,
                IsBuy = true,
                WalletId = _walletId,
                Amount = _amount,
                AssetId = _assetId
            }).Wait();
        }

        [Test]
        public void AlgoTrades_TestSaveAlgoInstanceTradeAsync()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            storage.Setup(s => s.InsertAsync(It.IsAny<AlgoInstanceTradeEntity>()))
                .Returns((AlgoInstanceTradeEntity entity, int[] parameters) =>
                {
                    CheckIfTradesEntityIsCorrect(entity);
                    return Task.CompletedTask;
                });

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            repository.SaveAlgoInstanceTradeAsync(new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                OrderId = _orderId,
                IsBuy = true,
                WalletId = _walletId,
                Amount = _amount,
                AssetId = _assetId,
                OrderStatus = Models.Enumerators.OrderStatus.UnknownStatus,
                OrderType = Models.Enumerators.OrderType.Unknown
            }).Wait();
        }

        [Test]
        public async Task AlgoTrades_TestGetAlgoInstanceOrderAsync_ReturnNull()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            storage.Setup(s => s.GetDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string partionKey, string rowKey) =>
                {
                    Assert.AreEqual(_orderId, partionKey);
                    Assert.AreEqual(_walletId, rowKey);

                    return Task.FromResult<AlgoInstanceTradeEntity>(null);
                });

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            var result = await repository.GetAlgoInstanceOrderAsync(_orderId, _walletId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task AlgoTrades_TestGetAlgoInstaceTradesByTradedAssetAsync_ReturnNotNull()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            var fakeQuery = new TableQuery<AlgoInstanceTradeEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    String.Format(_instanceId + "_" + _assetId)))
                .Take(_itemCount);

            storage.Setup(s => s.ExecuteAsync(It.IsAny<TableQuery<AlgoInstanceTradeEntity>>(),
                    It.IsAny<Action<IEnumerable<AlgoInstanceTradeEntity>>>(), It.IsAny<Func<bool>>()))
                .Returns((TableQuery<AlgoInstanceTradeEntity> query,
                    Action<IEnumerable<AlgoInstanceTradeEntity>> items, Func<bool> stop) =>
                {
                    Assert.AreEqual(query.TakeCount, fakeQuery.TakeCount);
                    Assert.AreEqual(query.FilterString, query.FilterString);
                    return Task.FromResult(GetAlgoInstanceEntities());
                })
                .Callback<TableQuery<AlgoInstanceTradeEntity>,
                    Action<IEnumerable<AlgoInstanceTradeEntity>>,
                    Func<bool>>((query, items, stop) => { items(GetAlgoInstanceEntities()); });

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            var result = await repository.GetAlgoInstaceTradesByTradedAssetAsync(_instanceId, _assetId, _itemCount);

            Assert.IsNotNull(result);
            CheckIfAlgoInstanceModelIsCorrect(result.First());
        }

        [Test]
        public async Task AlgoTrades_TestGetAlgoInstanceOrderAsync_ReturnNotNull()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            storage.Setup(s => s.GetDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetAlgoInstanceEntity()));

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            var result = await repository.GetAlgoInstanceOrderAsync(_orderId, _walletId);

            Assert.IsNotNull(result);
            CheckIfAlgoInstanceModelIsCorrect(result);
        }

        [Test]
        [Explicit("Should run manually only. Manipulate data in local Table Storage")]
        public void AlgoTrades_CreateAlgoInstanceOrderAsync_Test()
        {
            var logFactory = new Mock<ILogFactory>();
            var repository = new AlgoInstanceTradeRepository(
                AzureTableStorage<AlgoInstanceTradeEntity>.Create(GetDataStorageConnectionString(),
                    AlgoInstanceTradeRepository.TableName, logFactory.Object));

            var order = _fixture.Build<AlgoInstanceTrade>().With(x => x.OrderId, "TEST_ORDERID")
                .With(x => x.WalletId, "TEST_WALLETID").With(x => x.OrderType, OrderType.Limit).Create();

            repository.CreateAlgoInstanceOrderAsync(order).Wait();
        }

        [Test]
        [Explicit("Should run manually only. Manipulate data in local Table Storage")]
        public void AlgoTrades_GetAlgoInstanceOrderAsync_WillReturnValidData_Test()
        {
            var logFactory = new Mock<ILogFactory>();
            var repository = new AlgoInstanceTradeRepository(
                AzureTableStorage<AlgoInstanceTradeEntity>.Create(GetDataStorageConnectionString(),
                    AlgoInstanceTradeRepository.TableName, logFactory.Object));

            var order = repository.GetAlgoInstanceOrderAsync("TEST_ORDERID", "TEST_WALLETID").Result;

            Assert.IsNotNull(order);
        }

        #region  Unit Tests Helpers

        private AlgoInstanceTrade GetAlgoInstanceModel()
        {
            AlgoInstanceTrade fakeResult = new AlgoInstanceTrade()
            {
                Id = _walletId,
                InstanceId = _instanceId,
                OrderId = _orderId,
                IsBuy = true,
                WalletId = _walletId,
                Amount = _amount,
                AssetId = _assetId
            };

            return fakeResult;
        }

        private void CheckIfOrderEntityIsCorrect(AlgoInstanceTradeEntity entityToCheck)
        {
            AlgoInstanceTradeEntity fakeResult = GetAlgoInstanceEntity();

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
            string serializedSecond = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedSecond, serializedFirst);
        }

        private void CheckIfAlgoInstanceModelIsCorrect(AlgoInstanceTrade modelToCheck)
        {
            AlgoInstanceTrade fakeResult = GetAlgoInstanceModel();

            string serializedFirst = JsonConvert.SerializeObject(modelToCheck);
            string serializedSecond = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedSecond, serializedFirst);
        }

        private void CheckIfTradesEntityIsCorrect(AlgoInstanceTradeEntity entityToCheck)
        {
            AlgoInstanceTradeEntity fakeResult = new AlgoInstanceTradeEntity()
            {
                PartitionKey = _instanceId + "_" + _assetId,
                //Have to take the Rowkey of current entity because the RowKey is generated
                //with date time now and it won't be the same.
                RowKey = entityToCheck.RowKey,
                InstanceId = _instanceId,
                OrderId = _orderId,
                IsBuy = true,
                WalletId = _walletId,
                Amount = _amount,
                AssetId = _assetId,
                OrderStatus = Models.Enumerators.OrderStatus.UnknownStatus,
                OrderType = Models.Enumerators.OrderType.Unknown
            };

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
            string serializedSecond = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedSecond, serializedFirst);
        }

        private static IEnumerable<AlgoInstanceTrade> WhenInvokeGetTrades(AlgoInstanceTradeRepository repository,
            string instanceId, string assetId)
        {
            return repository.GetAlgoInstaceTradesByTradedAssetAsync(instanceId, assetId, 100).Result;
        }

        private static void WhenInvokeCreateEntity(AlgoInstanceTradeRepository repository, AlgoInstanceTrade entity)
        {
            repository.SaveAlgoInstanceTradeAsync(entity).Wait();
        }

        private static AlgoInstanceTradeRepository GivenAlgoInstanceTradeRepository()
        {
            return new AlgoInstanceTradeRepository(
                AzureTableStorage<AlgoInstanceTradeEntity>.Create(
                    SettingsMock.GetLogsConnectionString(), AlgoInstanceTradeRepository.TableName, new LogMock()));
        }

        private IEnumerable<AlgoInstanceTradeEntity> GetAlgoInstanceEntities()
        {
            List<AlgoInstanceTradeEntity> resultList = new List<AlgoInstanceTradeEntity>();
            resultList.Add(GetAlgoInstanceEntity());

            return resultList;
        }

        private AlgoInstanceTradeEntity GetAlgoInstanceEntity()
        {
            AlgoInstanceTradeEntity fakeResult = new AlgoInstanceTradeEntity()
            {
                PartitionKey = _orderId,
                RowKey = _walletId,
                InstanceId = _instanceId,
                OrderId = _orderId,
                IsBuy = true,
                WalletId = _walletId,
                Amount = _amount,
                AssetId = _assetId
            };

            return fakeResult;
        }

        private static IReloadingManager<AppSettings> InitConfig()
        {
            var reloadingMock = new Mock<IReloadingManager<AppSettings>>();

            reloadingMock.Setup(x => x.CurrentValue)
                .Returns(new AppSettings
                {
                    CSharpAlgoTemplateService = new CSharpAlgoTemplateSettings
                    {
                        Db = new DbSettings
                        {
                            TableStorageConnectionString = "UseDevelopmentStorage=true",
                            LogsConnString = "UseDevelopmentStorage=true"
                        }
                    }
                });

            var config = reloadingMock.Object;

            return config;
        }

        private static IReloadingManager<string> GetDataStorageConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.CSharpAlgoTemplateService.Db.TableStorageConnectionString);
        }

        #endregion
    }
}
