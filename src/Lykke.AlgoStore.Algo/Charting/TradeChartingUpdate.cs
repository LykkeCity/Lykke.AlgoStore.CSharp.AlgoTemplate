using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class TradeChartingUpdate : AlgoInstanceTrade, IChartingUpdate
    {
        public new string InstanceId { get; set; }
    }
}
