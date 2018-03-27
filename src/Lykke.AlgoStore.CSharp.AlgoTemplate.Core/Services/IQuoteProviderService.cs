using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IQuoteProviderService : IStopable, IStartable
    {
        Task Initialize();
        void Subscribe(string assetPair, Func<IAlgoQuote, Task> action);
    }
}
