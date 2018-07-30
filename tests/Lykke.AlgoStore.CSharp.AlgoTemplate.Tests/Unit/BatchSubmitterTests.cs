using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.Service.Logging.Client.AutorestClient.Models;
using NSubstitute;
using NUnit.Framework;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class BatchSubmitterTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void Batch_Persistance_Is_Not_Triggered_If_There_Are_No_Entries()
        {
            Task failure(object[] items)
            {
                Assert.Fail();
                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<object>(TimeSpan.FromMilliseconds(100), 1, failure))
            {
                Task.Delay(200).Wait();
            }
        }

        [Test]
        public void Batch_Persistance_Is_Not_Triggered_If_Entries_Amount_Not_Reached_And_Batch_Lifetime_Is_Expired()
        {
            var shouldSucceed = false;

            Task failure(object[] items)
            {
                Assert.IsTrue(shouldSucceed);
                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<object>(TimeSpan.FromHours(1), 100, failure))
            {
                var data = _fixture.CreateMany<object>(90);

                foreach (var obj in data)
                {
                    submitter.Enqueue(obj);
                }

                Task.Delay(50);

                shouldSucceed = true;
            }
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Entries_Amount()
        {
            var shouldSucceed = false;

            Task failure(object[] items)
            {
                Assert.AreEqual(100, items.Length);
                shouldSucceed = true;
                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<object>(TimeSpan.FromHours(1), 100, failure))
            {
                var data = _fixture.CreateMany<object>(100);

                foreach (var obj in data)
                {
                    submitter.Enqueue(obj);
                }

                Task.Delay(50).Wait();

                Assert.IsTrue(shouldSucceed);
            }
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Time_Expiration()
        {
            var shouldSucceed = false;

            Task write(DateTime[] times)
            {
                Assert.AreEqual(1, times.Length);

                foreach (var time in times)
                {
                    Assert.That(DateTimeOffset.UtcNow - time, Is.InRange(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1300)));
                }

                shouldSucceed = true;
                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<DateTime>(TimeSpan.FromSeconds(1), 100, write))
            {
                submitter.Enqueue(DateTime.UtcNow);

                Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            }

            Assert.IsTrue(shouldSucceed);
        }

        [Test]
        public void Batch_Persistance_Is_Triggered_By_Disposing()
        {
            var shouldSucceed = false;

            Task write(object[] data)
            {
                Assert.AreEqual(1, data.Length);

                shouldSucceed = true;
                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<object>(TimeSpan.FromHours(1), 10, write))
            {
                submitter.Enqueue(new object());
            }

            Assert.IsTrue(shouldSucceed);
        }

        [Test]
        public void Batch_Persistence_Is_Triggered_By_Entries_Amount_Extends_Batch_Lifetime()
        {
            var batchTimes = new List<DateTime>();
            var batchCounts = new List<int>();

            Task write(object[] items)
            {
                batchTimes.Add(DateTime.UtcNow);
                batchCounts.Add(items.Length);

                return Task.CompletedTask;
            }

            using (var submitter = new BatchSubmitter<object>(TimeSpan.FromSeconds(1), 10, write))
            {
                var data = _fixture.CreateMany<UserLogRequest>(9);

                foreach (var userLogRequest in data)
                {
                    submitter.Enqueue(userLogRequest);
                }

                //Wait half of a batch lifetime
                Task.Delay(500).Wait();

                //Complete batch
                submitter.Enqueue(new UserLogRequest());

                //Perform a batch
                Task.Delay(50).Wait();

                //Fill new batch
                submitter.Enqueue(new UserLogRequest());

                //Let batch time elapse
                Task.Delay(2000).Wait();

                Assert.AreEqual(2, batchTimes.Count);
                Assert.AreEqual(10, batchCounts.First());
                Assert.AreEqual(1, batchCounts.Last());
            }
        }
    }
}
