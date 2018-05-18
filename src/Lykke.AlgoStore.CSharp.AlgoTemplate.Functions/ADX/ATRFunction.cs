using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.ADX
{
    public class ATRFunction : IFunction
    {
        private readonly int _period;
        private Candle _previousInput;
        private int _samples { get; set; }

        private AtrParameters _functionParams = new AtrParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;

        public bool IsReady => _samples > _period + 1;

        public double? Value => currentATRValue;

        /// <summary>
        /// TR - True range
        /// </summary>
        private double TrueRange { get; set; }

        /// <summary>
        /// True ranges for the initial calculation of Avarage True Range
        /// </summary>
        private Queue<double> TrueRanges { get; set; }

        /// <summary>
        /// ATR
        /// </summary>
        private double? currentATRValue { get; set; }

        /// <summary>
        /// The Previous Average True Range which will be used for the calculation of the new one
        /// </summary>
        private double? PreviousAverageTrueRange { get; set; }


        public ATRFunction(AtrParameters adxParameters)
        {
            _period = adxParameters.Period;
            _functionParams = adxParameters;
            TrueRanges = _functionParams.Period == 0 ? new Queue<double>() : new Queue<double>(_functionParams.Period);
            currentATRValue = 0.0d;
        }

        public double? WarmUp(IEnumerable<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            var isFirstElement = true;

            foreach (var value in values)
            {
                _samples++;

                TrueRange = ComputeTrueRange(value);
                if (!isFirstElement)
                {
                    TrueRanges.Enqueue(TrueRange);
                }

                isFirstElement = false;

                currentATRValue = CalculateAverageTrueRange();
                _previousInput = value;
            }
            return currentATRValue;
        }

        public double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!IsReady)
                _samples++;

            TrueRange = ComputeTrueRange(value);

            if (!IsReady)
            {
                if (_samples > 1)
                {
                    TrueRanges.Enqueue(TrueRange);
                }
            }

            currentATRValue = CalculateAverageTrueRange();
            _previousInput = value;

            return currentATRValue;
        }

        /// <summary>
        /// Computes the True Range value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private double ComputeTrueRange(Candle input)
        {
            var trueRange = 0.0d;

            if (_previousInput == null) return trueRange;

            trueRange = Math.Max(Math.Abs(input.High - input.Low),
                            Math.Max(Math.Abs(input.High - _previousInput.Close),
                                        Math.Abs(input.Low - _previousInput.Close)));

            return trueRange;
        }

        /// <summary>
        /// Calculates the Avearage True Range
        /// </summary>
        /// <returns>
        /// Returns the new Avarage True Range
        /// </returns>
        private double? CalculateAverageTrueRange()
        {
            double? value = null;

            if (_samples == _period + 1)
            {
                value = TrueRanges.Average();
            }
            else if (_samples > _period + 1)
            {
                value = ((PreviousAverageTrueRange * (_period - 1)) + TrueRange) / _period;
            }

            PreviousAverageTrueRange = value;
            return value;
        }
    }
}
