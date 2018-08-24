using Lykke.AlgoStore.Algo;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface ICurrentDataProvider
    {
        IAlgoCandle CurrentCandle { get; set; }
        IAlgoQuote CurrentQuote { get; set; }

        DateTime CurrentTimestamp { get; set; }
        double CurrentPrice { get; set; }
    }
}
