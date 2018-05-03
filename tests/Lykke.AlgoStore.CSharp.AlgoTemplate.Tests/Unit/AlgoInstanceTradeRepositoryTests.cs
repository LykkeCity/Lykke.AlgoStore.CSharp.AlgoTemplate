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

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class AlgoInstanceTradeRepositoryTests
    {
        private AlgoInstanceTrade _entity;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperModelProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void GetAllAlgoInstanceTrade()
        {
            var instanceId = SettingsMock.GetInstanceId();
            var repo = GivenAlgoInstanceTradeRepository();
            var assetId = Guid.NewGuid().ToString();

            _entity = new AlgoInstanceTrade
            {
                InstanceId = instanceId,
                Amount = 2,
                IsBuy = true,
                Price = 2
            };

            WhenInvokeCreateEntity(repo, _entity);

            var statistics = WhenInvokeGetStatistics(repo, instanceId, assetId);

            Assert.AreEqual(1, statistics.ToList().Count);
        }

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
    }
}
