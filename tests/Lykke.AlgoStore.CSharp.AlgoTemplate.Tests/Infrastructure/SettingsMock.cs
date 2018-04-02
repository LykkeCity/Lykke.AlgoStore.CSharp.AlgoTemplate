using System;
using System.IO;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure
{
    public static class SettingsMock
    {
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
                            LogsConnString = "Mock connectionString",
                            TableStorageConnectionString = "Mock connectionString"
                        },
                        QuoteRabbitMqSettings = new QuoteRabbitMqSubscriptionSettings(),
                    },
                    MatchingEngineClient = new MatchingEngineSettings(),
                    AssetsServiceClient = new AssetsServiceClient(),
                    FeeCalculatorServiceClient = new FeeCalculatorServiceClient(),
                    FeeSettings = new FeeSettings()
                }
            );
            return reloadingMock.Object;
        }

        public static IReloadingManager<string> GetLogsConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.CSharpAlgoTemplateService.Db.LogsConnString);
        }

        public static IReloadingManager<string> GetTableStorageConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.CSharpAlgoTemplateService.Db.TableStorageConnectionString);
        }

        public static string GetInstanceId()
        {
            var settingsService = InitSettingsService();

            return settingsService.GetSetting("InstanceId");
        }

        public static AlgoInstanceType GetInstanceType()
        {
            var settingsService = InitSettingsService();

            return settingsService.GetInstanceType();
        }

        public static string GetAlgoId()
        {
            var settingsService = InitSettingsService();

            return settingsService.GetSetting("AlgoId");
        }

        public static string GetClientId()
        {
            var settingsService = InitSettingsService();

            return settingsService.GetAlgoInstanceClientId();
        }

        public static string GetWalletId()
        {
            var settingsService = InitSettingsService();

            return settingsService.GetAlgoInstanceWalletId();
        }

        public static IReloadingManager<QuoteRabbitMqSubscriptionSettings> GetQuoteSettings()
        {
            var config = InitConfig();

            return config.Nested(x => x.CSharpAlgoTemplateService.QuoteRabbitMqSettings);
        }

        private static IReloadingManager<AppSettings> InitConfig()
        {
            IReloadingManager<AppSettings> config;

            if (File.Exists(FileName))
                config = InitConfigurationFromFile();
            else
                config = InitMockConfiguration();
            return config;
        }

        public static IAlgoSettingsService InitSettingsService()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"5967bd68-1cea-4c24-9314-2fea325e0296\", \"InstanceId\": \"db216005-c8a7-486a-8cdc-037b179c8b74\", \"InstanceType\": \"Test\" }");

            var result = new AlgoSettingsService(Given_AlgoClientInstance_Repository());
            result.Initialize();

            return result;
        }

        private static AlgoClientInstanceRepository Given_AlgoClientInstance_Repository()
        {
            return new AlgoClientInstanceRepository(AzureTableStorage<AlgoClientInstanceEntity>.Create(
                SettingsMock.GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()));
        }
    }
}
