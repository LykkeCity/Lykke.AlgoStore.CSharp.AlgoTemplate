﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    public class MacdTrendAlgo : BaseAlgo
    {
        public double HoldingStep { get; set; }
        public double Tolerance { get; set; }

        private MACD _macd;
        private double _holdings;

        public double Holdings => _holdings;

        public override void OnStartUp()
        {
            _macd = MACD("MACD");
        }

        public override void OnCandleReceived(ICandleContext context)
        {
            var histogram = _macd.Histogram;
            var fast = _macd.Fast.Value;

            if (histogram == null || fast == null)
                return;

            var signalDeltaPercent = histogram / fast;

            context.Actions.Log($"Function values are: Macd histogram: {histogram}, Macd fast EMA: {fast}");

            if (signalDeltaPercent > Tolerance)
            {
                // TODO: Implement buying/selling when trading logic is finalized
                //context.Actions.BuyStraight(HOLDINGS_STEP);
                _holdings += HoldingStep;

                context.Actions.Log($"{signalDeltaPercent} is above {Tolerance}, buying {HoldingStep} more of the asset. " +
                                    $"Current holdings: {_holdings}");
            }
            else if (_holdings > 0 && signalDeltaPercent < -Tolerance)
            {
                // TODO: Implement buying/selling when trading logic is finalized
                //context.Actions.SellStraight(_holdings);
                _holdings = 0;

                context.Actions.Log($"{signalDeltaPercent} is below {-Tolerance}, selling all holdings.");
            }
        }
    }
}
