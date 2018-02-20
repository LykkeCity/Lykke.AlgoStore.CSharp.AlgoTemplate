using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    /// <summary>
    /// REMARK: Just a dummy algo implementation for future reference.
    /// We can and will remove this when first algo is implemented :)
    /// </summary>
    public class DummyAlgo : IAlgo
    {
        public void OnQuoteReceived(IContext context)
        {       
            var quote = context.Data.GetQuote();
            context.Actions.Log($"Receiving quote at {DateTime.UtcNow} " +
                $"{{quote.Price: {quote.Price}}}, {{quote.Timestamp: {quote.Timestamp}}}, " +
                $"{{quote.IsBuy: {quote.IsBuy}}}, {{quote.IsOnline: {quote.IsOnline}}}");

            var smaShort = context.Functions.GetValue("SMA_Short");
            var smaLong = context.Functions.GetValue("SMA_Long");
            context.Actions.Log($"Function values are: SMA_Short: {smaShort}, SMA_Long: {smaLong}");
            
            //var buyOrder = context.Actions.Buy(0.2);
            //context.Actions.Log($"Buying {buyOrder} at {DateTime.UtcNow}");
        }
    }
}
