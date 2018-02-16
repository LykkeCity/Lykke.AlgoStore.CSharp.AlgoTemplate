﻿namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class CSharpAlgoTemplateSettings
    {
        public DbSettings Db { get; set; }
        public string InstanceId { get; set; }
        public QuoteRabbitMqSubscriptionSettings QuoteRabbitMqSettings { get; set; }
        public MatchingEngineSettings MatchingEngineClient { get; set; }
        public FeeSettings FeeSettings { get; set; }
    }
}
