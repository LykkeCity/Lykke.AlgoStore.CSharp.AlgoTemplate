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
using AzureStorage;
using Moq;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class AlgoInstanceTradeRepositoryTests
    {
        private AlgoInstanceTrade _entity;

        private readonly string _instanceId = "17169b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _orderId = "11269b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _walletId = "66269b36-a51c-4f8c-8d17-09a45f0f4bc6";
        private readonly string _assetId = "USD";
        private readonly double _amount = 10;

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

            var statistics = WhenInvokeGetStatistics(repo, _instanceId, _assetId);
            Assert.AreEqual(1, statistics.ToList().Count);
        }

        [Test]
        public void AlgoTrades_TestGeneratePartitionKeyByInstanceIdAndAssetId()
        {
            string correctResult = _instanceId + "_" + _assetId;
            string resultToTest = AlgoInstanceTradeRepository.GeneratePartitionKeyByInstanceIdAndAssetId(_instanceId, _assetId);

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
            repository.CreateAlgoInstanceOrderAsync(new AlgoInstanceTrade()
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
                AssetId = _assetId
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
        public async Task AlgoTrades_TestGetAlgoInstanceOrderAsync_ReturnNotNull()
        {
            var storage = new Mock<INoSQLTableStorage<AlgoInstanceTradeEntity>>();

            storage.Setup(s => s.GetDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetAlgoInstanceEntity()));

            AlgoInstanceTradeRepository repository = new AlgoInstanceTradeRepository(storage.Object);
            var result = await repository.GetAlgoInstanceOrderAsync(_orderId, _walletId);

            Assert.IsNotNull(result);
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

        private void CheckIfOrderEntityIsCorrect(AlgoInstanceTradeEntity entityToCheck)
        {
            AlgoInstanceTradeEntity fakeResult = GetAlgoInstanceEntity();

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
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
                AssetId = _assetId
            };

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
            string serializedSecond = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedSecond, serializedFirst);
        }

        #region  Unit Tests Helpers

        private static IEnumerable<AlgoInstanceTrade> WhenInvokeGetStatistics(AlgoInstanceTradeRepository repository, string instanceId, string assetId)
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

        #endregion
    }
}
