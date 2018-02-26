using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.EMA;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MACD
{
    /// <summary>
    /// An implementation of the Moving Average Convergence/Divergence (MACD) indicator using Exponential Moving Averages (EMA)
    /// </summary>
    public class MacdFunction : AbstractFunction
    {
        private readonly MacdParameters _functionParams;

        private readonly EmaFunction _fastEma;
        private readonly EmaFunction _slowEma;
        private readonly EmaFunction _signalLine;

        /// <summary>
        /// The function parameters used to initialize this function
        /// </summary>
        public override FunctionParamsBase FunctionParameters => _functionParams;

        /// <summary>
        /// Initializes an instance of <see cref="MacdFunction"/>
        /// </summary>
        /// <param name="macdParameters">The parameters this function is going to make use of</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="macdParameters"/> is null</exception>
        public MacdFunction(MacdParameters macdParameters)
        {
            _functionParams = macdParameters ?? throw new ArgumentNullException(nameof(macdParameters));

            _fastEma = new EmaFunction(new EmaParameters { EmaPeriod = _functionParams.FastEmaPeriod });
            _slowEma = new EmaFunction(new EmaParameters { EmaPeriod = _functionParams.SlowEmaPeriod });
            _signalLine = new EmaFunction(new EmaParameters { EmaPeriod = _functionParams.SignalLinePeriod });
        }

        /// <summary>
        /// Adds a new value to be used in the function's calculations
        /// </summary>
        /// <param name="value">The new value to add</param>
        /// <returns>The new value of the function if the data is enough, null otherwise</returns>
        public override double? AddNewValue(double value)
        {
            var fastEmaValue = _fastEma.AddNewValue(value);
            var slowEmaValue = _slowEma.AddNewValue(value);

            if (slowEmaValue == null || fastEmaValue == null)
                return null;

            var macdLine = fastEmaValue - slowEmaValue;

            var signalLineValue = _signalLine.AddNewValue(macdLine.Value);

            if (signalLineValue == null)
                return null;

            return macdLine - signalLineValue;
        }

        /// <summary>
        /// Warm up the function with initial data
        /// </summary>
        /// <param name="values">The values to warm up the function with</param>
        /// <returns>The value of the function if the initial data was enough, null otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null</exception>
        public override double? WarmUp(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            double? result = null;

            foreach (var value in values)
                result = AddNewValue(value);

            return result;
        }
    }
}
