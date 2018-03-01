using System;
using System.Collections.Generic;
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
        private string _instanceId;
        private UserLog _entity;
        private static bool _entitySaved, _entitiesSaved;
        private List<UserLog> _entities;

        [SetUp]
        public void SetUp()
        {
            _instanceId = SettingsMock.GetInstanceId();

            _entity = new UserLog
            {
                InstanceId = _instanceId,
                Date = DateTime.UtcNow,
                Message = "User log message TEST!!!"
            };

            _entities = new List<UserLog>
            {
                new UserLog
                {
                    InstanceId = _instanceId,
                    Date = DateTime.UtcNow,
                    Message = "Multiple user log messages TEST - 1"
                },
                new UserLog
                {
                    InstanceId = _instanceId,
                    Date = DateTime.UtcNow,
                    Message = "Multiple user log messages TEST - 2"
                }
                ,new UserLog
                {
                    InstanceId = _instanceId,
                    Date = DateTime.UtcNow,
                    Message = "Multiple user log messages TEST - 3"
                },
                new UserLog
                {
                    InstanceId = _instanceId,
                    Date = DateTime.UtcNow,
                    Message = "Multiple user log messages TEST - 4"
                },
                new UserLog
                {
                    InstanceId = _instanceId,
                    Date = DateTime.UtcNow,
                    Message = "Multiple user log messages TEST - 5"
                }
            };

            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

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

            if (_entitiesSaved)
            {
                _entitiesSaved = false;
            }

            _entities = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void UserLog_Write_Test()
        {
            var repo = Given_UserLog_Repository();
            When_Invoke_Write(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void UserLog_Write_MultipleLogs_Test()
        {
            var repo = Given_UserLog_Repository();
            When_Invoke_Write(repo, _entities);
            Then_Data_ShouldBe_Saved(repo, _entities);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void UserLogWithCustomMessage_Write_Test()
        {
            var repo = Given_UserLog_Repository();
            When_Invoke_Write(repo, "Custom message sent as string");
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void UserLogWithCustomException_Write_Test()
        {
            var repo = Given_UserLog_Repository();

            try
            {
                throw new Exception("Custom message sent as exception");
            }
            catch (Exception ex)
            {
                When_Invoke_Write(repo, ex);
            }
        }

        private void When_Invoke_Write(UserLogRepository repository, Exception exception)
        {
            repository.WriteAsync(_instanceId, exception).Wait();
        }

        private void When_Invoke_Write(UserLogRepository repository, string message)
        {
            repository.WriteAsync(_instanceId, message).Wait();
        }

        private static void Then_Data_ShouldBe_Saved(UserLogRepository repo, UserLog entity)
        {
            Assert.NotNull(entity);
        }

        private static void Then_Data_ShouldBe_Saved(UserLogRepository repo, List<UserLog> entities)
        {
            Assert.NotNull(entities);

            var instanceId = SettingsMock.GetInstanceId();
            entities.Reverse();

            var task = repo.GetEntries(5, instanceId);
            task.Wait();

            var data = task.Result;

            for (int i = 0; i < entities.Count; i++)
            {
                Assert.AreEqual(data[i].InstanceId, entities[i].InstanceId);
                Assert.AreEqual(data[i].Date, entities[i].Date);
                Assert.AreEqual(data[i].Message, entities[i].Message);
            }
        }

        private static void When_Invoke_Write(UserLogRepository repository, UserLog data)
        {
            repository.WriteAsync(data).Wait();
            _entitySaved = true;
        }

        private static void When_Invoke_Write(UserLogRepository repository, List<UserLog> data)
        {
            foreach (var userLog in data)
            {
                repository.WriteAsync(userLog).Wait();
            }

            _entitiesSaved = true;
        }

        private static UserLogRepository Given_UserLog_Repository()
        {
            return new UserLogRepository(
                AzureTableStorage<UserLogEntity>.Create(SettingsMock.GetLogsConnectionString(), UserLogRepository.TableName, new LogMock())
            );
        }
    }
}
