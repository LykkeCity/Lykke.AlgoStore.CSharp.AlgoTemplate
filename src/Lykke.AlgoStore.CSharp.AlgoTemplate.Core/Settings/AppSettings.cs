using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.SlackNotifications;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings
{
    public class AppSettings
    {
        public CSharpAlgoTemplateSettings CSharpAlgoTemplateService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MatchingEngineAdapterSettings MatchingEngineAdapterClient { get; set; }
        public CandlesHistoryServiceClient CandlesHistoryServiceClient { get; set; }
    }
}
