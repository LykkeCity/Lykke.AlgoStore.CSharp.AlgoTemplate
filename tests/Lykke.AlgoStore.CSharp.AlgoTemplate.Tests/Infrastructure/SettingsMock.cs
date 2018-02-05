using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure
{
    public static class SettingsMock
    {
        //private static string Conn =
        //        "DefaultEndpointsProtocol=https;AccountName=algostoredev;AccountKey=d2VaBHrf8h8t622KvLeTPGwRP4Dxz9DTPeBT9H3zmjcQprQ1e+Z6Sx9RDqJc+zKwlSO900fzYF2Dg6oUBVDe1w=="
        //    ;

        private static readonly string FileName = "appsettings.Development.json";

        public static IReloadingManager<AppSettings> InitConfigurationFromFile()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            config.Providers.First().Set("SettingsUrl", "appsettings.Development.json");
            config.Providers.First().Set("ASPNETCORE_ENVIRONMENT", "Development");
            return config.LoadSettings<AppSettings>();
        }

        public static IReloadingManager<AppSettings> InitMockConfiguration()
        {
            var reloadingMock = new Mock<IReloadingManager<AppSettings>>();
            reloadingMock.Setup(x => x.CurrentValue).Returns
            (
                new AppSettings
                {
                    CSharpAlgoTemplateService = new CSharpAlgoTemplateSettings
                    {
                        Db = new DbSettings
                        {
                            LogsConnString = "Mock connectionString"
                        }
                    }
                }
            );
            return reloadingMock.Object;
        }

        public static IReloadingManager<string> GetSettings()
        {
            IReloadingManager<AppSettings> config;

            if (File.Exists(FileName))
                config = InitConfigurationFromFile();
            else
                config = InitMockConfiguration();

            return config.ConnectionString(x => x.CSharpAlgoTemplateService.Db.LogsConnString);


        }
    }
}
