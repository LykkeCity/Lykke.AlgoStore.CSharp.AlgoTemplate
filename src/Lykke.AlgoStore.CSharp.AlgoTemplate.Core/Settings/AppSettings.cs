using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.SlackNotifications;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Service.InstanceEventHandler.Client;
using Lykke.AlgoStore.Service.Logging.Client;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings
{
    public class AppSettings
    {
        public CSharpAlgoTemplateSettings CSharpAlgoTemplateService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MatchingEngineAdapterSettings MatchingEngineAdapterClient { get; set; }
        public HistoryServiceClientSettings HistoryServiceClient { get; set; }
        public LoggingServiceClientSettings AlgoStoreLoggingServiceClient { get; set; }
        public AlgoStoreStoppingClientSettings AlgoStoreStoppingClient { get; set; }
        public InstanceEventHandlerServiceClientSettings InstanceEventHandlerServiceClient { get; set; }
    }
}
