using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface ILimitOrderUpdateSubscriber : IDisposable
    {
        Task Subscribe(string instanceId, Action<AlgoInstanceTrade> updatesCallBack);
    }
}
