using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models
{
    public class AzureRepoFactories
    {
        public static StatisticsRepository CreateStatisticsRepository(IReloadingManager<string> connectionString,
            ILog log)
        {
            return new StatisticsRepository(AzureTableStorage<StatisticsSummaryEntity>.Create(connectionString, StatisticsRepository.TableName, log));
        }

        public static AlgoInstanceTradeRepository CreateAlgoTradeRepository(IReloadingManager<string> connectionString,
            ILog log)
        {
            return new AlgoInstanceTradeRepository(
                AzureTableStorage<AlgoInstanceTradeEntity>.Create(connectionString, AlgoInstanceTradeRepository.TableName, log));
        }

        public static AlgoClientInstanceRepository CreateAlgoClientInstanceRepository(IReloadingManager<string> connectionString,
            ILog log)
        {
            return new AlgoClientInstanceRepository(
                AzureTableStorage<AlgoClientInstanceEntity>.Create(connectionString, AlgoClientInstanceRepository.TableName, log),
                AzureTableStorage<AlgoInstanceStoppingEntity>.Create(connectionString, AlgoClientInstanceRepository.TableName, log));
        }
    }
}
