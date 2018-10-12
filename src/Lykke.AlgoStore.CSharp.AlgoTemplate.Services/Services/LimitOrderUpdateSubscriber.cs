using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders;
using Lykke.Common.Log;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class LimitOrderUpdateSubscriber : ILimitOrderUpdateSubscriber
    {
        private readonly ISubscriber _redisSubscriber;
        private readonly ConnectionMultiplexer _connection;
        private readonly NewtonsoftSerializer _jsonDeserializer;
        private readonly ILog _log;

        public LimitOrderUpdateSubscriber(string endPoint, string password, ILog log)
        {
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { endPoint },
                Password = password,
                Ssl = false,
                AbortOnConnectFail = false,
                AllowAdmin = false,
                ReconnectRetryPolicy = new LinearRetry(5000),
                CommandMap = CommandMap.Create(new HashSet<string>
                {
                    "PUBLISH" // ensures read-only mode
                }, available: false)
            };

            _connection = ConnectionMultiplexer.Connect(configurationOptions);
            _redisSubscriber = _connection.GetSubscriber();

            _log = log;
            _jsonDeserializer = new NewtonsoftSerializer();
        }


        public async Task Subscribe(string instanceId, Action<AlgoInstanceTrade> updatesCallBack)
        {
            try
            {
                await _redisSubscriber.SubscribeAsync(new RedisChannel(instanceId, RedisChannel.PatternMode.Auto), (channel, message) =>
                {
                    AlgoInstanceTrade orderUpdate = null;
                    try
                    {
                        orderUpdate = _jsonDeserializer.Deserialize<AlgoInstanceTrade>(message);
                        updatesCallBack(orderUpdate);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"Error while receiving limit order update for instanceId {instanceId}", orderUpdate);
                    }

                }, CommandFlags.PreferSlave);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error while subscribing for limit order updates for instanceId {instanceId}", instanceId);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            try
            {
                _redisSubscriber?.UnsubscribeAll();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error while unsubscribing and disposing redis subscription.");
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~LimitOrderUpdateSubscriber()
        {
            ReleaseUnmanagedResources();
        }
    }
}
