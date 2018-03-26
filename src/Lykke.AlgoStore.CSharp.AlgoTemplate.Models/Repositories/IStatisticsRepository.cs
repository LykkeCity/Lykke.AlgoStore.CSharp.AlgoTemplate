﻿using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IStatisticsRepository
    {
        Task CreateAsync(Statistics data);
        Task DeleteAsync(string instanceId, AlgoInstanceType instanceType, string id);
        Task DeleteAllAsync(string instanceId, AlgoInstanceType instanceType);
        Task<StatisticsSummary> GetSummary(string instanceId, AlgoInstanceType instanceType);
        Task CreateSummary(StatisticsSummary data);
    }
}
