using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoInstanceTcBuildEntity : TableEntity
    {
        public string InstanceId { get; set; }

        public string ClientId { get; set; }

        public string TcBuildId => RowKey;
    }
}
