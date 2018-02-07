using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entitites
{
    public class StatisticsEntity : TableEntity
    {
        public string InstanceId => PartitionKey;

        public string Id => RowKey;

        public bool IsBought { get; set; }

        public double Price { get; set; }

        public double Amount { get; set; }
    }
}
