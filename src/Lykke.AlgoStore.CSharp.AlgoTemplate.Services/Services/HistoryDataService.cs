﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using System.Collections.Generic;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Algo;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// Mostly dummy class, main logic of the history data service is 
    /// in the <see cref="CandleHistoryBatchEnumerator"/>
    /// </summary>
    public class HistoryDataService : IHistoryDataService
    {
        private readonly IHistoryClient _historyClient;

        public HistoryDataService(IHistoryClient historyClient)
        {
            _historyClient = historyClient;
        }

        public IEnumerable<Candle> GetHistory(CandlesHistoryRequest request)
        {
            return new CandleHistoryBatchEnumerable(request, _historyClient);
        }
    }
}
