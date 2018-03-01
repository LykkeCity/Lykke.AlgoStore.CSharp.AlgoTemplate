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
        public IList<Candle> GetHistory(CandlesHistoryRequest request)
        {
            return new List<Candle>()
            {
                    new Candle(){Open = 10.23, High = 44.52, Low = 43.97, Close= 44.51, DateTime = DateTime.UtcNow.AddDays(-30) },                    new Candle(){Open = 56.26, High = 44.92, Low = 44.35, Close=44.64 , DateTime = DateTime.UtcNow.AddDays(-29)},
                    new Candle(){Open = 40.23, High = 45.39, Low = 44.69, Close=45.22 , DateTime = DateTime.UtcNow.AddDays(-28)},
                    new Candle(){Open = 60.36, High = 45.70, Low = 45.13, Close=45.45 , DateTime = DateTime.UtcNow.AddDays(-27)},
                    new Candle(){Open = 60.99, High = 45.63, Low = 44.88, Close=45.49 , DateTime = DateTime.UtcNow.AddDays(-28)},
                    new Candle(){Open = 56.50, High = 45.52, Low = 44.19, Close=44.23 , DateTime = DateTime.UtcNow.AddDays(-27)},
                    new Candle(){Open = 55.26, High = 44.70, Low = 43.99, Close=44.61 , DateTime = DateTime.UtcNow.AddDays(-26)},
                    new Candle(){Open = 60.12, High = 45.15, Low = 43.75, Close=45.15 , DateTime = DateTime.UtcNow.AddDays(-25)},
                    new Candle(){Open = 70.36, High = 45.65, Low = 44.45, Close=44.53 , DateTime = DateTime.UtcNow.AddDays(-24)},
                    new Candle(){Open = 84.23, High = 45.87, Low = 45.13, Close=45.66 , DateTime = DateTime.UtcNow.AddDays(-23)},
                    new Candle(){Open = 56.26, High = 45.99, Low = 45.27, Close=45.95 , DateTime = DateTime.UtcNow.AddDays(-22)},
                    new Candle(){Open = 40.23, High = 46.35, Low = 45.80, Close=46.33 , DateTime = DateTime.UtcNow.AddDays(-21)},
                    new Candle(){Open = 60.36, High = 46.61, Low = 46.10, Close=46.31 , DateTime = DateTime.UtcNow.AddDays(-20)},
                    new Candle(){Open = 60.99, High = 46.47, Low = 45.77, Close=45.94 , DateTime = DateTime.UtcNow.AddDays(-19)},
                    new Candle(){Open = 56.50, High = 46.30, Low = 45.14, Close=45.60 , DateTime = DateTime.UtcNow.AddDays(-18)},
                    new Candle(){Open = 55.44, High = 45.98, Low = 44.96, Close=45.70 , DateTime = DateTime.UtcNow.AddDays(-17)},
                    new Candle(){Open = 56.26, High = 46.68, Low = 46.10, Close=46.56 , DateTime = DateTime.UtcNow.AddDays(-16)},
                    new Candle(){Open = 40.23, High = 46.59, Low = 46.14, Close=46.36 , DateTime = DateTime.UtcNow.AddDays(-15)},
                    new Candle(){Open = 60.36, High = 46.88, Low = 46.39, Close=46.82 , DateTime = DateTime.UtcNow.AddDays(-14)},
                    new Candle(){Open = 60.99, High = 46.81, Low = 46.41, Close=46.72 , DateTime = DateTime.UtcNow.AddDays(-12)},
                    new Candle(){Open = 56.50, High = 46.74, Low = 45.94, Close=46.65 , DateTime = DateTime.UtcNow.AddDays(-11)},
                    new Candle(){Open = 55.44, High = 47.08, Low = 46.68, Close=46.97 , DateTime = DateTime.UtcNow.AddDays(-10)},
                    new Candle(){Open = 56.26, High = 46.84, Low = 46.17, Close=46.56 , DateTime = DateTime.UtcNow.AddDays(-9) },
                    new Candle(){Open = 40.23, High = 45.81, Low = 45.10, Close=45.29 , DateTime = DateTime.UtcNow.AddDays(-8) },
                    new Candle(){Open = 60.36, High = 45.13, Low = 44.34, Close=44.93 , DateTime = DateTime.UtcNow.AddDays(-7) },
                    new Candle(){Open = 60.99, High = 44.95, Low = 44.60, Close=44.61 , DateTime = DateTime.UtcNow.AddDays(-6) },
                    new Candle(){Open = 56.50, High = 45.00, Low = 44.19, Close=44.69 , DateTime = DateTime.UtcNow.AddDays(-5) },                    new Candle(){Open = 55.44, High = 45.67, Low = 44.92, Close=45.26 , DateTime = DateTime.UtcNow.AddDays(-4) },
                    new Candle(){Open = 75.32, High = 45.71, Low = 45.00, Close=45.44 , DateTime = DateTime.UtcNow.AddDays(-3) },
                    new Candle(){Open = 80.36, High = 45.35, Low = 44.45, Close=44.75 , DateTime = DateTime.UtcNow.AddDays(-2) }
            };
        }
    }
}
