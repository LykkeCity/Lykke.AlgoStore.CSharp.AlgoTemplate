using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entitites;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories
{
    public class AzureRepoFactories
    {
        public static StatisticsRepository CreateStatisticsRepository(IReloadingManager<string> connectionString,
            ILog log)
        {
            return new StatisticsRepository(
                AzureTableStorage<StatisticsEntity>.Create(connectionString, StatisticsRepository.TableName, log));
        }
    }
}
