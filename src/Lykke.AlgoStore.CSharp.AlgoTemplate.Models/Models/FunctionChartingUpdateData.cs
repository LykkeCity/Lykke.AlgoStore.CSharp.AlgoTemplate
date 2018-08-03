using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class FunctionChartingUpdateData
    {
        public string FunctionName { get; set; }
        public double Value { get; set; }
        public string InstanceId { get; set; }
        public DateTime CalculatedOn { get; set; }
        public List<FunctionChartingUpdateData> InnerFunctions { get; set; }
    }
}
