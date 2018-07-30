﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using System;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    /// <summary>
    /// REMARK: Just a dummy algo implementation for future reference.
    /// We can and will remove this when first algo is implemented :)
    /// </summary>
    public class DummyAlgo : BaseAlgo
    {
        public SMA _shortSma { get; set; }
        public SMA _longSma { get; set; }

        public override void OnStartUp()
        {
            _shortSma = SMA("SMA_Short", 1, null, null, "BTCUSD", AlgoTemplate.Models.Enumerators.CandleTimeInterval.Minute, CandleOperationMode.CLOSE);
            _longSma = SMA("SMA_Long", 1, null, null, "BTCUSD", AlgoTemplate.Models.Enumerators.CandleTimeInterval.Minute, CandleOperationMode.CLOSE);
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

            if (quote.Price < 10000)
            {
                context.Actions.Buy(quote, Volume);
            }

            if (quote.Price > 8000)
            {
                context.Actions.Sell(quote, Volume);
            }
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
            //    context.Actions.Buy(context.Data.Candle, Volume);
            //}

            //if (quote.Price > 7000)
            //{
            context.Actions.Sell(context.Data.Candle, Volume);
            //}
        }
    }
}
