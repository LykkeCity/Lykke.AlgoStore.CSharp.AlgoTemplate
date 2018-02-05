using System;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class UserLogRepositoryTests
    {
        private const string AlgoId = "5fc5562b-79aa-4b23-977a-5c2e93018978";
        private UserLog _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _entity = new UserLog
            {
                AlgoId = AlgoId,
                Date = DateTime.UtcNow,
                Message = "Test user log message"
            };

            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_UserLog_Repository();

            if (_entitySaved)
            {
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only.Manipulate data in Table Storage")]
        public void UserLog_Write_Test()
        {
            var repo = Given_UserLog_Repository();
            When_Invoke_Write(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        private static void Then_Data_ShouldBe_Saved(UserLogRepository repo, UserLog entity)
        {
            Assert.NotNull(entity);
        }

        private static void When_Invoke_Write(UserLogRepository repository, UserLog data)
        {
            repository.WriteAsync(data).Wait();
            _entitySaved = true;
        }

        private static UserLogRepository Given_UserLog_Repository()
        {
            return new UserLogRepository(
                AzureTableStorage<UserLogEntity>.Create(SettingsMock.GetSettings(), UserLogRepository.TableName, new LogMock())
            );
        }
    }
}
