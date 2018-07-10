using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoInstanceStoppingEntity : TableEntity
    {
        public string AlgoInstanceStatusValue { get; set; }

        public string InstanceId { get; set; }

        public string ClientId { get; set; }

        public string EndOnDateTicks => RowKey;

        public AlgoInstanceStatus AlgoInstanceStatus
        {
            get
            {
                AlgoInstanceStatus type = 0;
                Enum.TryParse(AlgoInstanceStatusValue, out type);
                return type;
            }
            set => AlgoInstanceStatusValue = value.ToString();
        }
    }
}
