using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class PredefinedHistoryDataService : IHistoryDataService
    {
        public IList<Candle> GetTestCandles(string externalDataFilename)
        {
            List<Candle> candles = new List<Candle>();

            bool first = true;
            foreach (var line in File.ReadLines(Path.Combine("PredefinedData", externalDataFilename)))
            {
                var parts = line.Split(',');
                if (first)
                {
                    first = false;
                    continue;
                }
                candles.Add(new Candle
                {
                    Open = Convert.ToDouble(parts[0]),
                    High = Convert.ToDouble(parts[1]),
                    Low = Convert.ToDouble(parts[2]),
                    Close = Convert.ToDouble(parts[3]),
                    DateTime = Convert.ToDateTime(parts[4]),
                });
            }
            return candles;
        }

        public IList<Candle> GetHistory(CandlesHistoryRequest request)
        {
            return GetTestCandles("CandlesData.txt");
        }
    }
}
