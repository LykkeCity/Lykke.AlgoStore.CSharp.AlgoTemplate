using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    internal static class SettingsExtensions
    {
        public static RabbitMqSubscriptionSettings ToRabbitMqSettings(this QuoteRabbitMqSubscriptionSettings quoteSettings)
        {
            if (quoteSettings == null)
                return null;

            var result = RabbitMqSubscriptionSettings.CreateForSubscriber(
                quoteSettings.ConnectionString,
                quoteSettings.NamespaceOfSourceEndpoint,
                quoteSettings.NameOfSourceEndpoint,
                quoteSettings.NamespaceOfEndpoint,
                quoteSettings.NameOfEndpoint);

            if (quoteSettings.IsDurable)
                result.MakeDurable();

            result.ReconnectionsCountToAlarm = quoteSettings.ReconnectionsCountToAlarm;
            result.ReconnectionDelay = TimeSpan.FromMilliseconds(quoteSettings.ReconnectionDelayInMSec);

            return result;
        }
    }
}
