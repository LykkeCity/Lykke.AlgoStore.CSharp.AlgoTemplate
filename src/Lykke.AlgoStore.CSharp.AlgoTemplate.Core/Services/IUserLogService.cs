﻿using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IUserLogService
    {
        Task Write(UserLog userLog);
        Task Write(string instanceId, Exception exception);
        Task Write(string instanceId, string message);
    }
}
