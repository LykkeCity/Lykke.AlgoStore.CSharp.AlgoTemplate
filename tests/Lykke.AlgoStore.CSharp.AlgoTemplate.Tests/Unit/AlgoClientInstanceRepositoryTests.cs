﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using System.Linq;

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
        private static DateTime _instanceEndOnDate;
        private static string _tcBuildId;

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();

            _instanceId = SettingsMock.GetInstanceId();
            _algoId = SettingsMock.GetAlgoId();
            _clientId = "123456clientId";
            _walletId = "123456walletId";
            _instanceEndOnDate = new DateTime(2018, 06, 19, 15, 30, 00, DateTimeKind.Utc);
            _tcBuildId = Guid.NewGuid().ToString();

            _entity = new AlgoClientInstanceData
            {
                InstanceId = _instanceId,
                TradedAssetId = "BTC",
                AlgoId = _algoId,
                AssetPairId = "BTCUSD",
                HftApiKey = "1234",
                Margin = 1,
                Volume = 1,
                ClientId = _clientId,
                WalletId = _walletId,
                AlgoInstanceType = AlgoInstanceType.Live,
                IsStraight = true,
                AlgoInstanceStatus = AlgoInstanceStatus.Started,
                InstanceName = "Unit Test",
                AlgoInstanceCreateDate = DateTime.UtcNow,
                AlgoMetaDataInformation = new AlgoMetaDataInformation()
                {
                    Parameters = new[] {new  AlgoMetaDataParameter()
                    {
                        Key = "Stopwatch",
                        Value = "Val 1",
                        Type = "String",
                        //ParameterType = "Lykke.AlgoStore.CSharp.Funct.SimpleMovingAverage"
                    },
                        new  AlgoMetaDataParameter()
                    {
                        Key = "EndOn",
                        Value = _instanceEndOnDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Type = "System.DateTime"
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
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);

            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoClientStoppingInstance_Save_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);

            When_Invoke_Save(repo, _entity);
            Then_Stopping_Entity_ShouldBe_Saved(repo);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoClientStoppingInstance_Delete_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);

            When_Invoke_Save(repo, _entity);
            Then_Stopping_Entity_ShouldBe_Saved(repo);

            When_Update_Instance_Status_To_Stopped(repo, _entity);
            Then_Stopping_Entity_ShouldBe_Deleted(repo);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoClientTcBuildInstance_Save_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);

            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);

            When_Invoke_Save_And_Add_TcBuild_Row(repo, _entity);
            Then_TcBuild_Entity_ShouldBe_Saved(repo);           
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoClientTcBuildInstance_Delete_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);

            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);

            When_Invoke_Save_And_Add_TcBuild_Row(repo, _entity);
            Then_TcBuild_Entity_ShouldBe_Saved(repo);

            When_Invoke_DeleteAlgoInstance(repo, _entity);
            Then_TcBuild_Entity_ShouldBe_Deleted(repo);
        }

        [Test]
        public void AlgoClientInstance_Get_Metadata_Setting_Test()
        {
            var repo = Given_AlgoClientInstance_Repository();
            var validationResult = ValidateData(_entity);
            Then_ValidationResult_ShouldBe_Empty(validationResult);
            When_Invoke_Save(repo, _entity);

            string clientIdValue = repo.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.ClientId;
            string walletIdValue = repo.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.WalletId;
            string assetPairValue = repo.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.AssetPairId;
            string assetValue = repo.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.TradedAssetId;
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
                SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()),
                AzureTableStorage<AlgoInstanceStoppingEntity>.Create(
                    SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()),
                AzureTableStorage<AlgoInstanceTcBuildEntity>.Create(
                    SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            data.Validate(null);
            repository.SaveAlgoInstanceDataAsync(data).Wait();          
            _entitySaved = true;
        }

        private static void When_Invoke_Save_And_Add_TcBuild_Row(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            data.Validate(null);
            data.TcBuildId = _tcBuildId;
            repository.SaveAlgoInstanceDataAsync(data).Wait();
            _entitySaved = true;
        }

        private void When_Update_Instance_Status_To_Stopped(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            data.AlgoInstanceStatus = AlgoInstanceStatus.Stopped;
            repository.SaveAlgoInstanceDataAsync(data).Wait();
            _entitySaved = true;
        }

        private static void When_Invoke_DeleteAlgoInstance(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            repository.DeleteAlgoInstanceDataAsync(data).Wait();
            _entitySaved = false;
        }

        private static void Then_Data_ShouldBe_Saved(AlgoClientInstanceRepository repository, AlgoClientInstanceData data)
        {
            var retrievedByAlgoId = repository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId).Result;
            var retrievedByClientId = repository.GetAlgoInstanceDataByClientIdAsync(data.ClientId, data.InstanceId).Result;
            var retrievedByAlgoIdAndInstanceType =
                repository.GetAllAlgoInstancesByAlgoIdAndInstanceTypeAsync(data.AlgoId, data.AlgoInstanceType).Result;

            Assert.NotNull(retrievedByAlgoId);
            Assert.NotNull(retrievedByClientId);
            Assert.NotNull(retrievedByAlgoIdAndInstanceType);

            var expectedJson = JsonConvert.SerializeObject(data);
            var actualRetrievedByAlgoIdJson = JsonConvert.SerializeObject(retrievedByAlgoId);
            var actualRetrievedByClientId = JsonConvert.SerializeObject(retrievedByClientId);
            var actualRetrievedByAlgoIdAndInstanceType = JsonConvert.SerializeObject(retrievedByAlgoIdAndInstanceType.FirstOrDefault());

            Assert.AreEqual(expectedJson, actualRetrievedByAlgoIdJson);
            Assert.AreEqual(expectedJson, actualRetrievedByClientId);
            Assert.AreEqual(expectedJson, actualRetrievedByAlgoIdAndInstanceType);
        }

        private IEnumerable<ValidationResult> ValidateData(AlgoClientInstanceData data)
        {
            return data.Validate(null);
        }

        private void Then_ValidationResult_ShouldBe_Empty(IEnumerable<ValidationResult> validationResults)
        {
            Assert.IsEmpty(validationResults);
        }

        private void Then_Stopping_Entity_ShouldBe_Saved(AlgoClientInstanceRepository repository)
        {
            var retrievedAllEntitiesWithPassedEndDate = repository.GetAllAlgoInstancesPastEndDate(_instanceEndOnDate.AddMinutes(5)).Result;

            Assert.IsTrue(retrievedAllEntitiesWithPassedEndDate.Any());

            Should_Be_Equal(retrievedAllEntitiesWithPassedEndDate.First());
        }
        
        private void Then_Stopping_Entity_ShouldBe_Deleted(AlgoClientInstanceRepository repository)
        {
            var retrievedAllEntitiesWithPassedEndDate = repository.GetAllAlgoInstancesPastEndDate(_instanceEndOnDate.AddMinutes(5)).Result;

            Assert.IsTrue(!retrievedAllEntitiesWithPassedEndDate.Any());
        }

        private void Then_TcBuild_Entity_ShouldBe_Saved(AlgoClientInstanceRepository repository)
        {
            var retrievedTcBuildEntity = repository.GetAlgoInstanceDataByTcBuildIdAsync(_tcBuildId).Result;

            Assert.IsNotNull(retrievedTcBuildEntity);

            Should_Be_Equal(retrievedTcBuildEntity);
        }

        private void Then_TcBuild_Entity_ShouldBe_Deleted(AlgoClientInstanceRepository repository)
        {
            var retrivedTcBuildEntity = repository.GetAlgoInstanceDataByTcBuildIdAsync(_tcBuildId).Result;

            Assert.IsNull(retrivedTcBuildEntity);
        }

        private void Should_Be_Equal(AlgoInstanceStoppingData data)
        {
            Assert.AreEqual(_instanceEndOnDate.Ticks, long.Parse(data.EndOnDateTicks));
            Assert.AreEqual(_entity.InstanceId, data.InstanceId);
            Assert.AreEqual(_entity.ClientId, data.ClientId);
        }

        private void Should_Be_Equal(AlgoInstanceTcBuildData data)
        {
            Assert.AreEqual(_entity.InstanceId, data.InstanceId);
            Assert.AreEqual(_entity.ClientId, data.ClientId);
            Assert.AreEqual(_tcBuildId, data.TcBuildId);
        }

        #endregion Private Methods
    }
}
