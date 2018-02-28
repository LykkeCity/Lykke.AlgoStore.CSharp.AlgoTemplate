using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MACD;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    public class MacdTrendAlgo : BaseAlgo
    {
        private MacdFunction _macd;
        private double _holdings;

        // TODO: Replace this with algo parameters when they're implemented
        private const double HOLDINGS_STEP = 2;
        private const double TOLERANCE = 0.0025;

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

            if(signalDeltaPercent > TOLERANCE)
            {
                context.Actions.BuyStraight(HOLDINGS_STEP);
                _holdings += HOLDINGS_STEP;

                context.Actions.Log($"{signalDeltaPercent} is above {TOLERANCE}, buying {HOLDINGS_STEP} more of the asset. " +
                                    $"Current holdings: {_holdings}");
            }
            else if(_holdings > 0 && signalDeltaPercent < -TOLERANCE)
            {
                context.Actions.SellStraight(_holdings);
                _holdings = 0;

                context.Actions.Log($"{signalDeltaPercent} is below {-TOLERANCE}, selling all holdings.");
            }
        }
    }
}
