namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class CSharpAlgoTemplateSettings
    {
        public DbSettings Db { get; set; }
        public QuoteRabbitMqSubscriptionSettings QuoteRabbitMqSettings { get; set; }
        public BaseRabbitMqSubscriptionSettings CandleRabbitMqSettings { get; set; }
        public MonitoringSettings MonitoringSettings { get; set; }
        public LoggingSettings EventHandlerSettings { get; set; }
        public LoggingSettings LoggingSettings { get; set; }
        public string AlgoNamespaceValue { get; set; }
        public RedisSettings Redis { get; set; }
    }
}
