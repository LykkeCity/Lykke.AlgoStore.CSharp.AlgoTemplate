using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.SlackNotifications;
using Lykke.AlgoStore.Service.Logging.Client;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings
{
    public class AppSettings
    {
        public CSharpAlgoTemplateSettings CSharpAlgoTemplateService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MatchingEngineAdapterSettings MatchingEngineAdapterClient { get; set; }
        public CandlesHistoryServiceClient CandlesHistoryServiceClient { get; set; }
        public LoggingServiceClientSettings AlgoStoreLoggingServiceClient { get; set; }
    }
}
