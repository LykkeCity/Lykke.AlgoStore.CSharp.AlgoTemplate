using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class RabbitMqQuoteProviderService : IQuoteProviderService
    {
        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Action<IAlgoQuote> action)
        {
            throw new NotImplementedException();
        }
    }
}
