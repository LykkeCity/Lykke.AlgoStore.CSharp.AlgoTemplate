using System;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;

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

            var settingsService = new AlgoSettingsService();

            Assert.Throws<ArgumentException>(settingsService.Initialize);
        }

        [Test]
        public void GetNoneExistingSetting_ForNonEmptyEnvironmentVariable_ShouldReturnEmptyString()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService();
            settingsService.Initialize();

            var result = settingsService.GetSetting("unknown");

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetExistingSetting_ForNonEmptyEnvironmentVariable_ShouldReturnValue()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456\", \"InstanceId\": \"654321\" }");

            var settingsService = new AlgoSettingsService();
            settingsService.Initialize();

            var result = settingsService.GetSetting("AlgoId");

            Assert.AreEqual(result, "123456");
        }
    }
}
