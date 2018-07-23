using System;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo.Indicators
{
    /// <summary>
    /// An implementation of the Moving Average Convergence/Divergence (MACD) function using Exponential Moving Averages (EMA)
    /// </summary>
    public class MACD : AbstractIndicator
    {
        private EMA _slowEma;
        private EMA _fastEma;
        private EMA _signalLine;

        private double? _currentValue;

        /// <summary>
        /// Gets the latest value of the function
        /// </summary>
        public override double? Value => _currentValue;

        public override bool IsReady => _currentValue != null;

        /// <summary>
        /// Returns the histogram component of the MACD function (MACD line - Signal line)
        /// </summary>
        public double? Histogram => _currentValue - Signal.Value;

        /// <summary>
        /// Returns the fast EMA component of the MACD function
        /// </summary>
        public EMA Fast => _fastEma;

        /// <summary>
        /// Returns the slow EMA component of the MACD function
        /// </summary>
        public EMA Slow => _slowEma;

        /// <summary>
        /// Returns the signal line component of the MACD function
        /// </summary>
        public EMA Signal => _signalLine;

        /// <summary>
        /// The slow moving EMA period
        /// </summary>
        public int SlowEmaPeriod => _slowEma.Period;

        /// <summary>
        /// The fast moving EMA period
        /// </summary>
        public int FastEmaPeriod => _slowEma.Period;

        /// <summary>
        /// The signal line EMA period
        /// </summary>
        public int SignalLinePeriod => _slowEma.Period;

        public MACD(
            int fastEmaPeriod,
            int slowEmaPeriod,
            int signalLinePeriod,
            DateTime startingDate,
            DateTime endingDate,
            CandleTimeInterval candleTimeInterval,
            string assetPair,
            CandleOperationMode candleOperationMode)
            : base(startingDate, endingDate, candleTimeInterval, assetPair, candleOperationMode)
        {
            SetEmaPeriod(fastEmaPeriod, ref _fastEma);
            SetEmaPeriod(slowEmaPeriod, ref _slowEma);
            SetEmaPeriod(signalLinePeriod, ref _signalLine);
        }

        /// <summary>
        /// Adds a new value to be used in the function's calculations
        /// </summary>
        /// <param name="value">The new value to add</param>
        /// <returns>The new value of the function if the data is enough, null otherwise</returns>
        public override double? AddNewValue(double value)
        {
            var fastEmaValue = Fast.AddNewValue(value);
            var slowEmaValue = Slow.AddNewValue(value);

            if (slowEmaValue == null || fastEmaValue == null)
                return null;

            var macdLine = fastEmaValue - slowEmaValue;

            var signalLineValue = Signal.AddNewValue(macdLine.Value);

            if (signalLineValue == null)
                return null;

            _currentValue = macdLine;

            return _currentValue;
        }

        /// <summary>
        /// Warm up the function with initial data
        /// </summary>
        /// <param name="values">The values to warm up the function with</param>
        /// <returns>The value of the function if the initial data was enough, null otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null</exception>
        public override double? WarmUp(IEnumerable<double> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            double? result = null;

            // Needed to properly initialize the EMA functions
            var emptyWarmup = new double[0];
            Fast.WarmUp(emptyWarmup);
            Slow.WarmUp(emptyWarmup);
            Signal.WarmUp(emptyWarmup);

            foreach (var value in values)
                result = AddNewValue(value);

            return result;
        }

        private void SetEmaPeriod(int period, ref EMA ema)
        {
            if (ema != null)
                throw new InvalidOperationException("Cannot set EMA period more than once");

            ema = new EMA(period, StartingDate, EndingDate, CandleTimeInterval, AssetPair, CandleOperationMode);
        }
    }
}
