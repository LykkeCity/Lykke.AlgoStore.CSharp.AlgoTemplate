using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IQuoteChartingUpdateRepository
    {
        Task WriteAsync(IEnumerable<QuoteChartingUpdateData> data);
        Task<IEnumerable<QuoteChartingUpdateData>> GetQuotesForPeriodAsync(string instanceId, string assetPair, DateTime from, DateTime to, CancellationToken ct, bool? isBuy = null);
        Task SaveDifferentPartionsAsync(IEnumerable<QuoteChartingUpdateData> data);
    }
}
