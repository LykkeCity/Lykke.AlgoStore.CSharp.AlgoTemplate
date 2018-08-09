using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class QuoteChartingUpdate : IAlgoQuote, IChartingUpdate
    {
        public bool IsBuy { get; }
        public double Price { get; }
        public DateTime Timestamp { get; }
        public DateTime DateReceived { get; }
        public bool IsOnline { get; }
        public string InstanceId { get; set; }
    }
}
