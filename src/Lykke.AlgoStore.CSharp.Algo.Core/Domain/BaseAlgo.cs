using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
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
    }
}
