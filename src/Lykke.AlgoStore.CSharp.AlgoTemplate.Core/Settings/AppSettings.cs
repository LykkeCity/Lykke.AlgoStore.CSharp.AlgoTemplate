using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.SlackNotifications;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings
{
    public class AppSettings
    {
        public CSharpAlgoTemplateSettings CSharpAlgoTemplateService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MatchingEngineSettings MatchingEngineClient { get; set; }
        public AssetsServiceClient AssetsServiceClient { get; set; }
        public CandlesHistoryServiceClient CandlesHistoryServiceClient { get; set; }
        public FeeSettings FeeSettings { get; set; }
        public FeeCalculatorServiceClient FeeCalculatorServiceClient { get; set; }
    }
}
