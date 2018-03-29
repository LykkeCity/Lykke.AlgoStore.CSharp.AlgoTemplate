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

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Infrastructure
{
    public static class SettingsMock
    {
        public static IReloadingManager<AppSettings> InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            config.Providers.First().Set("SettingsUrl", "http://40.114.150.117/613a49ba-21dc-4fce-b336-a60304f30e36_CSharpAlgoTemplateService");
            config.Providers.First().Set("ASPNETCORE_ENVIRONMENT", "Development");

            return config.LoadSettings<AppSettings>();
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
            var config = InitConfiguration();
            return config;
        }

        public static IAlgoSettingsService InitSettingsService()
        {
            Environment.SetEnvironmentVariable("ALGO_INSTANCE_PARAMS", "{ \"AlgoId\": \"123456789\", \"InstanceId\": \"123456789\", \"InstanceType\": \"Test\" }");

            var result = new AlgoSettingsService(Given_AlgoClientInstance_Repository());
            result.Initialize();

            return result;
        }

        private static AlgoClientInstanceRepository Given_AlgoClientInstance_Repository()
        {
            return new AlgoClientInstanceRepository(AzureTableStorage<AlgoClientInstanceEntity>.Create(
                GetTableStorageConnectionString(), AlgoClientInstanceRepository.TableName, new LogMock()));
        }
    }
}
