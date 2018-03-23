using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class StatisticsEntity : TableEntity
    {
        public string InstanceId { get; set; }

        public bool? IsBuy { get; set; }

        public double? Price { get; set; }

        public double? Amount { get; set; }

        public bool? IsStarted { get; set; }

        public string AlgoInstanceTypeValue { get; set; }

        public AlgoInstanceType InstanceType
        {
            get
            {
                Enum.TryParse(AlgoInstanceTypeValue, out AlgoInstanceType type);
                return type;
            }
            set => AlgoInstanceTypeValue = value.ToString();
        }
    }
}
