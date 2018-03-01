using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    public class AlgoClientInstanceRepositoryTests
    {
        private string _instanceId;
        private string _algoId;
        private string _clientId;
        private string _walletId;
        private AlgoClientInstanceData _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _instanceId = SettingsMock.GetInstanceId();
            _algoId = SettingsMock.GetAlgoId();
            _clientId = "123456clientId";
            _walletId = "123456walletId";

            _entity = new AlgoClientInstanceData
            {
                InstanceId = _instanceId,
                TradedAsset = "BTC",
                AlgoId = _algoId,
                AssetPair = "BTCUSD",
                HftApiKey = "1234",
                Margin = 1,
                Volume = 1,
                ClientId = _clientId,
                WalletId = _walletId,
                AlgoMetaDataInformation = new AlgoMetaDataInformation()
                {
                    Parameters = new[] {new  AlgoMetaDataParameter()
                    {
                        Key = "Stopwatch",
                        Value = "Val 1",
                        Type = "String",
                        FunctionType = "Lykke.AlgoStore.CSharp.Funct.SimpleMovingAverage"
                    }}
                }
            };
        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoClientInstance_Repository();

            if (_entitySaved)
            {
                repo.DeleteAlgoInstanceDataAsync(_entity).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoClientInstance_Save_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test]
        public void AlgoClientInstance_Get_Metadata_Setting_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            When_Invoke_Save(repo, _entity);

            string clientIdValue = repo.GetAlgoInstanceClientId(_algoId, _instanceId).Result;
            string walletIdValue = repo.GetAlgoInstanceWalletId(_algoId, _instanceId).Result;
            string assetPairValue = repo.GetAlgoInstanceAssetPair(_algoId, _instanceId).Result;
            string assetValue = repo.GetAlgoInstanceTradedAsset(_algoId, _instanceId).Result;
            string metadataInfo = repo.GetAlgoInstanceMetadataSetting(_algoId, _instanceId, "Stopwatch").Result;

            string expectedClientId = _clientId;
            string expectedWalletId = _walletId;
            string expectedAssetPair = "BTCUSD";
            string expectedAsset = "BTC";
            string expectedMetadataInfo = "Val 1";

            Assert.AreEqual(expectedClientId, clientIdValue);
            Assert.AreEqual(expectedWalletId, walletIdValue);
            Assert.AreEqual(expectedAssetPair, assetPairValue);
            Assert.AreEqual(expectedAsset, assetValue);
            Assert.AreEqual(metadataInfo, expectedMetadataInfo);
        }

        #region Private Methods

        private static AlgoClientInstanceRepository Given_AlgoClientInstance_Repository()
        {
            return new AlgoClientInstanceRepository(AzureTableStorage<AlgoClientInstanceEntity>.Create(
                SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            repository.SaveAlgoInstanceDataAsync(data).Wait();
            _entitySaved = true;
        }

        private static void Then_Data_ShouldBe_Saved(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            var retrievedByAlgoId = repository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId).Result;
            var retrievedByClientId = repository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId).Result;

            Assert.NotNull(retrievedByAlgoId);
            Assert.NotNull(retrievedByClientId);

            var expectedJson = JsonConvert.SerializeObject(data);
            var actualRetrievedByAlgoIdJson = JsonConvert.SerializeObject(retrievedByAlgoId);
            var actualRetrievedByClientId = JsonConvert.SerializeObject(retrievedByClientId);

            Assert.AreEqual(expectedJson, actualRetrievedByAlgoIdJson);
            Assert.AreEqual(expectedJson, actualRetrievedByClientId);          
        }

        #endregion Private Methods
    }
}
