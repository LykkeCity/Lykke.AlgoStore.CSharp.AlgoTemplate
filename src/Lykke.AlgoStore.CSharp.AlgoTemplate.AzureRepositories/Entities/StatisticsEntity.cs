using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities
{
    public class StatisticsEntity : TableEntity
    {
        public string InstanceId => PartitionKey;

        public string Id => RowKey;

        public bool? IsBuy { get; set; }

        public double? Price { get; set; }

        public double? Amount { get; set; }

        public bool? IsStarted { get; set; }
    }
}
