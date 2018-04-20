﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoInstanceTradeRepository
    {
        Task<AlgoInstanceTrade> GetAlgoInstanceOrderAsync(string orderId, string walletId);
        Task CreateAlgoInstanceOrderAsync(AlgoInstanceTrade product);
    }
}
