using Autofac;
using Common;
using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface ICandleProviderService : IStopable, IStartable
    {
        Task Initialize();
        void Subscribe(string assetPair, CandleTimeInterval timeInterval, Action<Candle> callback);
        void SetPrevCandleFromHistory(string assetPair, CandleTimeInterval timeInterval, Candle candle);
    }
}
