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

            var smaShort = _shortSma.Value;
            var smaLong = _longSma.Value;
            context.Actions.Log($"Function values are: SMA_Short: {smaShort}, SMA_Long: {smaLong}");

            //if (quote.Price < 10000)
            //{
            context.Actions.Buy(Volume);
            Console.Write("Buy " + Volume);
            //}

            //if (quote.Price > 7000)
            //{
            //    context.Actions.Sell(Volume);
            //}
        }


        public override void OnCandleReceived(ICandleContext context)
        {
            context.Actions.Log($"Volume value: {Volume}");

            var candle = context.Data.Candle;
            context.Actions.Log($"Receiving candle at {candle.DateTime} candle close Price {candle.Close}");

            var smaShort = _shortSma.Value;
            var smaLong = _longSma.Value;
            context.Actions.Log($"Function values are: SMA_Short: {smaShort}, SMA_Long: {smaLong}");

            //if (quote.Price < 10000)
            //{
            //context.Actions.Buy(context.Data.Candle, Volume);
            //}

            //if (quote.Price > 7000)
            //{
            context.Actions.Sell(context.Data.Candle, Volume);
            //}
        }
    }
}
