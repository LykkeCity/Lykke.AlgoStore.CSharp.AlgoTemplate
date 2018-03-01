using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Entities
{
    public class UserLogEntity : TableEntity
    {
        public string InstanceId => PartitionKey;

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}
