using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class CandleChartingUpdate : Candle, IChartingUpdate
    {
        public string InstanceId { get; set; }
    }
}
