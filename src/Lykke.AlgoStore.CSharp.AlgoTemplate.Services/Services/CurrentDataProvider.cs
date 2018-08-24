using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class CurrentDataProvider : ICurrentDataProvider
    {
        public IAlgoCandle CurrentCandle { get; set; }
        public IAlgoQuote CurrentQuote { get; set; }
        public DateTime CurrentTimestamp { get; set; }
        public double CurrentPrice { get; set; }
    }
}
