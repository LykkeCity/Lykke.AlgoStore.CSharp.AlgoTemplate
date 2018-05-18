using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
{
    public class BaseAlgo : IAlgo
    {
        public virtual void OnCandleReceived(ICandleContext context)
        {
        }

        public virtual void OnQuoteReceived(IQuoteContext context)
        {
        }

        public virtual void OnStartUp(IFunctionProvider functions)
        {
        }

        public string AssetPair { get; set; }
        public CandleTimeInterval CandleInterval { get; set; }
        public DateTime StartFrom { get; set; }
        public DateTime EndOn { get; set; }
        public double Volume { get; set; }
        public string TradedAsset { get; set; }
    }
}
