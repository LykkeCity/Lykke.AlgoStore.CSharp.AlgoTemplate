namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class BaseRabbitMqSubscriptionSettings
    {
        // SettingsServer - SpotPricesRabbitMqConnectionString
        public string ConnectionString { get; set; }
        // Both namespaces are same - lykke. maybe use just one
        public string NamespaceOfSourceEndpoint { get; set; }
        public string NameOfSourceEndpoint { get; set; }
        // Both namespaces are same - lykke. maybe use just one
        public string NamespaceOfEndpoint { get; set; }
        public string NameOfEndpoint { get; set; }
        public bool IsDurable { get; set; }
        /// <summary>
        /// Lykke Default is 3 seconds
        /// </summary>
        public int ReconnectionDelayInMSec { get; set; }
        /// <summary>
        /// Count of silent reconnection attempts before write error message to the log. Lykke Default is - 20
        /// </summary>
        public int ReconnectionsCountToAlarm { get; set; }
    }
}
