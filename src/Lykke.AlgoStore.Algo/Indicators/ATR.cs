﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class ATR : BaseIndicator
    {
        /// <summary>
        /// True ranges for the initial calculation of Average True Range
        /// </summary>
        private readonly Queue<double> _trueRanges = new Queue<double>();

        /// <summary>
        /// The Previous Average True Range which will be used for the calculation of the new one
        /// </summary>
        private double? _previousAtr;
        private Candle _previousInput;
        private double _trueRange;

        private int _samples;
        private double? _value;

        public override bool IsReady => _samples > Period + 1;

        public override double? Value => _value;

        public int Period { get; }

        public ATR(
            int period,
            DateTime startingDate,
            DateTime endingDate,
            CandleTimeInterval candleTimeInterval,
            string assetPair)
            : base(startingDate, endingDate, candleTimeInterval, assetPair)
        {
            Period = period;
        }

        public override double? WarmUp(IEnumerable<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            var isFirstElement = true;

            foreach (var value in values)
            {
                _samples++;

                _trueRange = ComputeTrueRange(value);
                if (!isFirstElement)
                {
                    _trueRanges.Enqueue(_trueRange);
                }

                isFirstElement = false;

                _value = CalculateAverageTrueRange();
                _previousInput = value;
            }
            return _value;
        }

        public override double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!IsReady)
                _samples++;

            _trueRange = ComputeTrueRange(value);

            if (!IsReady)
            {
                if (_samples > 1)
                {
                    _trueRanges.Enqueue(_trueRange);
                }
            }

            _value = CalculateAverageTrueRange();
            _previousInput = value;

            return _value;
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

            if (_samples == Period + 1)
            {
                value = _trueRanges.Average();
            }
            else if (_samples > Period + 1)
            {
                value = ((_previousAtr * (Period - 1)) + _trueRange) / Period;
            }

            _previousAtr = value;
            return value;
        }
    }
}
