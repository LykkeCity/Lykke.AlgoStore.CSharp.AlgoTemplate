﻿namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class CSharpAlgoTemplateSettings
    {
        public DbSettings Db { get; set; }
        public QuoteRabbitMqSubscriptionSettings QuoteRabbitMqSettings { get; set; }
    }
}
