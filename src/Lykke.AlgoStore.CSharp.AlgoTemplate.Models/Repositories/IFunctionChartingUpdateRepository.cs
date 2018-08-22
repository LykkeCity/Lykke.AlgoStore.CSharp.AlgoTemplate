using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public  interface IFunctionChartingUpdateRepository
    {
        Task<IEnumerable<FunctionChartingUpdateData>> GetFunctionChartingUpdateForPeriodAsync(string instanceId, DateTime from, DateTime to, CancellationToken ct);
    }
}
