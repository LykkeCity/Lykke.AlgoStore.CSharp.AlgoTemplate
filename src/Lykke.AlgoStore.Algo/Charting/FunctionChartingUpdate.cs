using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class FunctionChartingUpdate : IChartingUpdate
    {
        public string FunctionName { get; set; }
        public double Value { get; set; }
        public string InstanceId { get; set; }
        public List<FunctionChartingUpdate> InnerFunctions { get; set; }
    }
}
