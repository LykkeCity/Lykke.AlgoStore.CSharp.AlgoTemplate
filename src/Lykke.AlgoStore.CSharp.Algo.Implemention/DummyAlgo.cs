﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.MatchingEngine.Connector.Models.Api;

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
        public int count { get; set; }

        public override void OnStartUp()
        {
            _shortSma = SMA("SMA_Short"/* period: 1, candleTimeInterval:CandleTimeInterval.Hour, candleOperationMode:CandleOperationMode.CLOSE*/ ); //example usage of inline arguments
            _longSma = SMA("SMA_Long"/* period: 1, candleTimeInterval:CandleTimeInterval.Hour, candleOperationMode:CandleOperationMode.CLOSE*/);
            count = 0;
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
                //context.Orders.Market.Create(OrderAction.Buy, Volume);
            }

            if (quote.Price > 8000)
            {
                //context.Orders.Market.Create(OrderAction.Sell, Volume);
            }

            if (count < 2)
            {
                //example create limit order and subscribe for events
                //var limitOrder = context.Orders.Limit.Create(OrderAction.Buy, 0.2, 6500);
                //limitOrder.OnRegistered += LimitOrderPlaced;
                count++;
            }

        }

        //limit order update example handlers
        private void LimitOrderPlaced(ILimitOrder order)
        {
            //action placed limit order
        }
        private void LimitOrderFulfilled(ILimitOrder order)
        {
            //action Fulfilled limit order
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
            //context.Orders.Market.Create(OrderAction.Sell, Volume);
            //}

            if (count < 2)
            {
                //example create limit order and subscribe for events
                //var limitOrder = context.Orders.Limit.Create(OrderAction.Buy, 0.2, 6500);
                //limitOrder.OnFulfilled += LimitOrderFulfilled;
                count++;
            }
        }
    }
}
