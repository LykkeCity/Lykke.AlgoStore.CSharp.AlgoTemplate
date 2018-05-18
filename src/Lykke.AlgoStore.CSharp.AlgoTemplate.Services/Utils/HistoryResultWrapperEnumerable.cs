using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using System.Collections;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Candles;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Dummy class which is needed to implement <see cref="IEnumerable{T}"/> and provide access to
    /// <see cref="HistoryResultWrapperEnumerator"/>
    /// </summary>
    public class HistoryResultWrapperEnumerable : IEnumerable<Candle>
    {
        private readonly IEnumerable<Candle> _originalEnumerable;
        private readonly ICandleProviderService _candleProvider;
        private readonly CandlesHistoryRequest _candlesHistoryRequest;
        private readonly CandlesService.SubscriptionData _subscriptionData;

        public HistoryResultWrapperEnumerable(IEnumerable<Candle> originalEnumerable,
                                              ICandleProviderService candleProvider,
                                              CandlesHistoryRequest candlesHistoryRequest,
                                              CandlesService.SubscriptionData subscriptionData)
        {
            _originalEnumerable = originalEnumerable;
            _candleProvider = candleProvider;
            _candlesHistoryRequest = candlesHistoryRequest;
            _subscriptionData = subscriptionData;
        }

        public IEnumerator<Candle> GetEnumerator()
        {
            return new HistoryResultWrapperEnumerator(_originalEnumerable.GetEnumerator(), _candleProvider,
                                                      _candlesHistoryRequest, _subscriptionData);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new HistoryResultWrapperEnumerator(_originalEnumerable.GetEnumerator(), _candleProvider,
                                                      _candlesHistoryRequest, _subscriptionData);
        }
    }
}
