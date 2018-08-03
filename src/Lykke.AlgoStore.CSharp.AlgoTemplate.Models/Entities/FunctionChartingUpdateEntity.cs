using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class FunctionChartingUpdateEntity : AzureTableEntity
    {
        public string FunctionName { get; set; }
        public double Value { get; set; }
        public string InstanceId { get; set; }
        public DateTime CalculatedOn { get; set; }

        [JsonValueSerializer]
        public List<FunctionChartingUpdateEntity> InnerFunctions { get; set; }
    }
}
