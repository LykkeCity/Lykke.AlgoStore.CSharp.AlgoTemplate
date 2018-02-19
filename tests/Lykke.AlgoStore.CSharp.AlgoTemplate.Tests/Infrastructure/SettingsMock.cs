﻿using System.IO;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
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
                            LogsConnString = "Mock connectionString"
                        },
                        InstanceId = "Mock InstanceId",
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

        public static IReloadingManager<string> GetInstanceId()
        {
            var config = InitConfig();

            return config.Nested(x => x.CSharpAlgoTemplateService.InstanceId);
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
    }
}
