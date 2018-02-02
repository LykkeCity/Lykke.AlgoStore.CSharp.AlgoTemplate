using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IQuoteProviderService
    {
        Task Initialize();
        void Subscribe(Action<IAlgoQuote> action);
    }
}
