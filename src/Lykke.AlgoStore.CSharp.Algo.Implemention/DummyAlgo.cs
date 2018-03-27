using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    /// <summary>
    /// REMARK: Just a dummy algo implementation for future reference.
    /// We can and will remove this when first algo is implemented :)
    /// </summary>
    public class DummyAlgo : BaseAlgo
    {
        private SmaFunction _shortSma;
        private SmaFunction _longSma;

        public override void OnStartUp(IFunctionProvider functions)
        {
            _shortSma = functions.GetFunction<SmaFunction>("SMA_Short");
            _longSma = functions.GetFunction<SmaFunction>("SMA_Long");
        }

        public override void OnQuoteReceived(IQuoteContext context)
        {
            context.Actions.Log($"Volume value: {Volume}");

            var quote = context.Data.Quote;
            context.Actions.Log($"Receiving quote at {DateTime.UtcNow} " +
                $"{{quote.Price: {quote.Price}}}, {{quote.Timestamp: {quote.Timestamp}}}, " +
                $"{{quote.IsBuy: {quote.IsBuy}}}, {{quote.IsOnline: {quote.IsOnline}}}");

            var smaShort = _shortSma.GetValue();
            var smaLong = _longSma.GetValue();
            context.Actions.Log($"Function values are: SMA_Short: {smaShort}, SMA_Long: {smaLong}");

            //if (quote.Price < 10000)
            //{
            //    context.Actions.Buy(Volume);
            //}

            //if (quote.Price > 8000)
            //{
            //    context.Actions.Sell(Volume);
            //}
        }

    }
}
