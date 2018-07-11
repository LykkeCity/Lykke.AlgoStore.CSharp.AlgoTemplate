using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class DMIMinus : IIndicator
    {
        private readonly int _period;
        private readonly bool _isAverageTrueRangeSet = false;

        private int _samples { get; set; }

        private double? _currentDMIMinusValue { get; set; }
        private double? SmoothedDirectionalMovementMinus { get; set; }
        private double? PreviousSmoothedDirectionalMovementMinus { get; set; }

        private Queue<double> DirectionalMovementMinuses { get; set; }
        private double DirectionalMovementMinus { get; set; }

        public bool IsReady => _samples > _period + 1;

        public double? Value => _currentDMIMinusValue;

        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public CandleTimeInterval CandleTimeInterval { get; set; }
        public string AssetPair { get; set; }

        /// <summary>
        /// ATR
        /// </summary>
        public double? AverageTrueRange { get; set; }
        public double? PreviousAverageTrueRange { get; set; }

        private Candle _previousInput;
        private ATR ATRFunction { get; set; }

        public DMIMinus(int period, bool isAverageTrueRangeSet = true)
        {
            _period = period;
            DirectionalMovementMinuses = _period == 0 ? new Queue<double>() : new Queue<double>(_period);
            _isAverageTrueRangeSet = isAverageTrueRangeSet;
            AverageTrueRange = 0.0d;
            ATRFunction = new ATR { Period = _period };
            _samples = 0;
        }

        public double? WarmUp(IEnumerable<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            var isFirstElement = true;

            foreach (var value in values)
            {
                _samples++;

                if (!_isAverageTrueRangeSet)
                {
                    AverageTrueRange = ATRFunction.AddNewValue(value);
                }

                DirectionalMovementMinus = ComputeNegativeDirectionalMovement(value);

                if (!isFirstElement && !IsReady)
                {
                    DirectionalMovementMinuses.Enqueue(DirectionalMovementMinus);
                }

                isFirstElement = false;

                SmoothedDirectionalMovementMinus = ComputeSmoothedDirectionalMovementMinus();
                _currentDMIMinusValue = ComputeNegativeDirectionalIndex();

                _previousInput = value;
            }

            return _currentDMIMinusValue;
        }

        public double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!IsReady)
                _samples++;

            if (!_isAverageTrueRangeSet)
            {
                AverageTrueRange = ATRFunction.AddNewValue(value);
            }

            DirectionalMovementMinus = ComputeNegativeDirectionalMovement(value);

            if (!IsReady)
            {
                if (_samples > 1)
                {
                    DirectionalMovementMinuses.Enqueue(DirectionalMovementMinus);
                }
            }

            SmoothedDirectionalMovementMinus = ComputeSmoothedDirectionalMovementMinus();
            _currentDMIMinusValue = ComputeNegativeDirectionalIndex();

            _previousInput = value;

            return _currentDMIMinusValue;
        }

        /// <summary>
        /// Computes the negative directional movement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private double ComputeNegativeDirectionalMovement(Candle input)
        {
            var negativeDirectionalMovement = 0.0d;

            if (_previousInput == null) return negativeDirectionalMovement;

            if ((_previousInput.Low - input.Low) > (input.High - _previousInput.High))
            {
                if ((_previousInput.Low - input.Low) > 0)
                {
                    negativeDirectionalMovement = _previousInput.Low - input.Low;
                }
            }
            return negativeDirectionalMovement;
        }

        /// <summary>
        /// Computes the Minus Directional Movment Indicator (-DMI period).
        /// </summary>
        /// <returns></returns>
        private double? ComputeNegativeDirectionalIndex()
        {
            if (AverageTrueRange == null || AverageTrueRange == 0) return AverageTrueRange;

            var negativeDirectionalIndex = (SmoothedDirectionalMovementMinus.Value / AverageTrueRange.Value) * 100;

            return negativeDirectionalIndex;
        }

        /// <summary>
        /// Calculates the Smoothed -DX
        /// </summary>
        private double? ComputeSmoothedDirectionalMovementMinus()
        {
            double? value = null;

            if (_samples == _period + 1)
            {
                value = DirectionalMovementMinuses.Average();
                PreviousSmoothedDirectionalMovementMinus = value.Value;
            }
            else if (_samples > _period + 1)
            {
                value = ((PreviousSmoothedDirectionalMovementMinus * (_period - 1)) + DirectionalMovementMinus) / _period;
                PreviousSmoothedDirectionalMovementMinus = value;
            }

            return value;
        }
    }
}
