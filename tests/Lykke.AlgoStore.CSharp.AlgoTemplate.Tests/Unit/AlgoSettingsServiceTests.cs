using System;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;
using Mapper = AutoMapper.Mapper;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class AlgoSettingsServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void Initialize_ForEmptyEnvironmentVariable_ShouldThrowException()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "");

            var settingsService = new AlgoSettingsService(Given_AlgoClientInstance_Repository());

            Assert.Throws<ArgumentException>(settingsService.Initialize);
        }

        [Test]
        public void GetNoneExistingSetting_ForNonEmptyEnvironmentVariable_ShouldReturnEmptyString()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService(Given_AlgoClientInstance_Repository());
            settingsService.Initialize();

            var result = settingsService.GetSetting("unknown");

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetExistingSetting_ForNonEmptyEnvironmentVariable_ShouldReturnValue()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService(Given_AlgoClientInstance_Repository());
            settingsService.Initialize();

            var result = settingsService.GetSetting("AlgoId");

            Assert.AreEqual(result, "123456");
        }

        [Test]
        public void IsAlive_ForNonInitializedService_ShouldReturnFalse()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService(Given_AlgoClientInstance_Repository());

            var result = settingsService.IsAlive();

            Assert.AreEqual(result, false);
        }

        [Test]
        public void IsAlive_ForInitializedService_ShouldReturnTrue()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService(Given_AlgoClientInstance_Repository());
            settingsService.Initialize();

            var result = settingsService.IsAlive();

            Assert.AreEqual(result, true);
        }

        private static AlgoClientInstanceRepository Given_AlgoClientInstance_Repository()
        {
            return new AlgoClientInstanceRepository(AzureTableStorage<AlgoClientInstanceEntity>.Create(
                SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()));
        }
    }
}
