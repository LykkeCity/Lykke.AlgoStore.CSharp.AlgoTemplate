using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class PredefinedHistoryDataService : IHistoryDataService
    {
        public IList<Candle> GetHistory(CandlesHistoryRequest request)
        {
            return new List<Candle>()
            {
                new Candle() {Open = 100, Close = 101.51, High = 102.14, Low = 99.45, DateTime = DateTime.UtcNow.AddDays(-6)},
                new Candle() {Open = 101.51, Close = 102.11, High = 102.11, Low = 101.45, DateTime = DateTime.UtcNow.AddDays(-5)},
                new Candle() {Open = 102.11, Close = 101.10, High = 102.11, Low = 100.55, DateTime = DateTime.UtcNow.AddDays(-4)},
                new Candle() {Open = 101.10, Close = 104.51, High = 105.6, Low = 101.10, DateTime = DateTime.UtcNow.AddDays(-3)},
                new Candle() {Open = 104.51, Close = 100.51, High = 104.51, Low = 99.45, DateTime = DateTime.UtcNow.AddDays(-2)},
                new Candle() {Open = 100.51, Close = 99.54, High = 101.21, Low = 99.45, DateTime = DateTime.UtcNow.AddDays(-1)}
            };
        }
    }
}
