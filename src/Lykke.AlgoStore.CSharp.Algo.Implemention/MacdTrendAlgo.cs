﻿using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MACD;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    public class MacdTrendAlgo : BaseAlgo
    {
        public double HoldingStep { get; set; }
        public double Tolerance { get; set; }

        private MacdFunction _macd;
        private double _holdings;

        // TODO: Replace this with algo parameters when they're implemented
        //private const double HOLDINGS_STEP = 2;
        //private const double TOLERANCE = 0.0025;

        public double Holdings => _holdings;

        public override void OnStartUp(IFunctionProvider functions)
        {
            _macd = functions.GetFunction<MacdFunction>("MACD");
        }

        public override void OnCandleReceived(ICandleContext context)
        {
            var histogram = _macd.Histogram;
            var fast = _macd.Fast.Value;

            if (histogram == null || fast == null)
                return;

            var signalDeltaPercent = histogram / fast;

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
                //context.Actions.SellStraight(_holdings);
                _holdings = 0;

                context.Actions.Log($"{signalDeltaPercent} is below {-Tolerance}, selling all holdings.");
            }
        }
    }
}
