using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IQuoteProviderService
    {
        void Initialize();
        void Subscribe(Action<IAlgoQuote> action);
    }
}
