using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entitites;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories
{
    public static class AzureRepoFactories
    {
        public static UserLogRepository CreateUserLogRepository(IReloadingManager<string> connectionString, ILog log)
        {
            return new UserLogRepository(
                AzureTableStorage<UserLogEntity>.Create(connectionString, UserLogRepository.TableName, log));
        }

        public static StatisticsRepository CreateStatisticsRepository(IReloadingManager<string> connectionString,
            ILog log)
        {
            return new StatisticsRepository(
                AzureTableStorage<StatisticsEntity>.Create(connectionString, StatisticsRepository.TableName, log));
        }
    }
}
