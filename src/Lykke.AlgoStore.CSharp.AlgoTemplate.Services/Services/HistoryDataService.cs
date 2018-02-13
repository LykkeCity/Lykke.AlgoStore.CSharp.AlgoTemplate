using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class HistoryDataService : IHistoryDataService
    {
        public IList<Candle> GetHistory(CandlesHistoryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
