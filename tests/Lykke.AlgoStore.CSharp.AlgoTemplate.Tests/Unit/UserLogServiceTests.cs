using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.Service.Logging.Client.AutorestClient.Models;
using NSubstitute;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class UserLogServiceTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void Batch_Persistance_Is_Not_Triggered_If_There_Are_No_Entries()
        {
            var userLogClient = Substitute.For<ILoggingClient>();

            using (new UserLogService(userLogClient, TimeSpan.FromMilliseconds(100), 1))
            {
                Task.Delay(200).Wait();

                userLogClient.DidNotReceive().WriteAsync("TEST_INSTANCE_ID", "TEST_INSTANCE_MESSAGE");
            }
        }

        [Test]
        public void Batch_Persistance_Is_Not_Triggered_If_Entries_Amount_Not_Reached_And_Batch_Lifetime_Is_Expired()
        {
            var userLogClient = Substitute.For<ILoggingClient>();

            using (var service = new UserLogService(userLogClient, TimeSpan.FromHours(1), 100))
            {
                var data = _fixture.CreateMany<UserLogRequest>(90);

                foreach (var userLogRequest in data)
                {
                    service.Enqueue(userLogRequest);
                }

                Task.Delay(50);

                userLogClient.DidNotReceive().WriteAsync(Arg.Any<IList<UserLogRequest>>());
            }
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Entries_Amount()
        {
            var userLogClient = Substitute.For<ILoggingClient>();

            using (var service = new UserLogService(userLogClient, TimeSpan.FromHours(1), 100))
            {
                var data = _fixture.CreateMany<UserLogRequest>(100);

                foreach (var userLogRequest in data)
                {
                    service.Enqueue(userLogRequest);
                }

                Task.Delay(50).Wait();

                userLogClient.Received(1).WriteAsync(Arg.Is<IList<UserLogRequest>>(x => x.Count == 100));
            }
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Time_Expiration()
        {
            var userLogClient = Substitute.For<ILoggingClient>();

            userLogClient.WriteAsync(Arg.Any<IList<UserLogRequest>>()).Returns(callInfo =>
            {
                var logEntities = callInfo.Arg<IList<UserLogRequest>>();

                foreach (var entity in logEntities)
                {
                    Assert.That(DateTimeOffset.UtcNow - entity.Date, Is.InRange(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1300)));
                }

                return Task.CompletedTask;
            });

            using (var service = new UserLogService(userLogClient, TimeSpan.FromSeconds(1), 100))
            {
                service.Enqueue(new UserLogRequest { Date = DateTime.UtcNow, InstanceId = "TEST_INSTANCE_ID", Message = "TEST_INSTANCE_MESSAGE" });

                Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                userLogClient.Received(1).WriteAsync(Arg.Is<IList<UserLogRequest>>(x => x.Count == 1));
            }
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Disposing()
        {
            var userLogClient = Substitute.For<ILoggingClient>();

            using (var service = new UserLogService(userLogClient, TimeSpan.FromHours(1), 10))
            {
                service.Enqueue(new UserLogRequest
                {
                    Message = "TEST_INSTANCE_MESSAGE",
                    Date = DateTime.UtcNow,
                    InstanceId = "TEST_INSTANCE_ID"
                });
            }

            userLogClient.Received(1).WriteAsync(Arg.Is<IList<UserLogRequest>>(x => x.Count == 1));
        }

        [Test]
        public void Batch_Persistence_Is_Triggered_By_Entries_Amount_Extends_Batch_Lifetime()
        {
            var userLogClient = Substitute.For<ILoggingClient>();
            var batchTimes = new List<DateTime>();
            var batchCounts = new List<int>();

            userLogClient.WriteAsync(Arg.Any<IList<UserLogRequest>>()).Returns(callInfo =>
            {
                batchTimes.Add(DateTime.UtcNow);
                batchCounts.Add(callInfo.Arg<IList<UserLogRequest>>().Count);

                return Task.CompletedTask;
            });

            using (var service = new UserLogService(userLogClient, TimeSpan.FromSeconds(1), 10))
            {
                var data = _fixture.CreateMany<UserLogRequest>(9);

                foreach (var userLogRequest in data)
                {
                    service.Enqueue(userLogRequest);
                }

                //Wait half of a batch lifetime
                Task.Delay(500).Wait();

                //Complete batch
                service.Enqueue(new UserLogRequest());

                //Perform a batch
                Task.Delay(50).Wait();

                //Fill new batch
                service.Enqueue(new UserLogRequest());

                //Let batch time elapse
                Task.Delay(2000).Wait();

                Assert.AreEqual(2, batchTimes.Count);
                Assert.AreEqual(10, batchCounts.First());
                Assert.AreEqual(1, batchCounts.Last());
            }
        }
    }
}
