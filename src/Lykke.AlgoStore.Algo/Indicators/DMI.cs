using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class DMI : BaseIndicator
    {
        private readonly int _period;
        private readonly bool _isAverageTrueRangeSet = false;

        private int _samples { get; set; }

        private double? _currentDownwardDirectionalIndex;
        private double? _currentUpwardDirectionalIndex;

        private double? _previousDownwardDirectionalIndex;

        private double? _previousUpwardDirectionalIndex;

        private Queue<double> _downwardDirectionalIndices;
        private Queue<double> _upwardDirectionalIndices;

        public override bool IsReady => _samples > _period + 1;

        public override double? Value => _currentDownwardDirectionalIndex;

        public double? UpwardDirectionalIndex => _currentUpwardDirectionalIndex;
        public double? DownwardDirectionalIndex => _currentDownwardDirectionalIndex;

        private Candle _previousInput;
        private ATR ATRFunction { get; set; }

        public DMI(
            int period,
            DateTime startingDate,
            DateTime endingDate,
            CandleTimeInterval candleTimeInterval,
            string assetPair)
            : base(startingDate, endingDate, candleTimeInterval, assetPair)
        {
            _period = period;
            _downwardDirectionalIndices = _period == 0 ? new Queue<double>() : new Queue<double>(_period);
            _upwardDirectionalIndices = _period == 0 ? new Queue<double>() : new Queue<double>(_period);
            ATRFunction = new ATR(period, startingDate, endingDate, candleTimeInterval, assetPair);
            _samples = 0;
        }

        public override double? WarmUp(IEnumerable<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
            {
                AddNewValue(value);
            }

            return _currentDownwardDirectionalIndex;
        }

        public override double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!IsReady)
                _samples++;

            ATRFunction.AddNewValue(value);

            var downwardDirectionalMovement = ComputeNegativeDirectionalMovement(value);
            var upwardDirectionalMovement = ComputePositiveDirectionalMovement(value);

            _currentDownwardDirectionalIndex = 
                ComputeDirectionalIndex(downwardDirectionalMovement,
                                        _downwardDirectionalIndices,
                                        ref _previousDownwardDirectionalIndex);

            _currentUpwardDirectionalIndex =
                ComputeDirectionalIndex(upwardDirectionalMovement,
                                        _upwardDirectionalIndices,
                                        ref _previousUpwardDirectionalIndex);

            _previousInput = value;

            return _currentDownwardDirectionalIndex;
        }

        private double? ComputeDirectionalIndex(
            double directionalMovement,
            Queue<double> indicesQueue,
            ref double? previousDirectionalIndex)
        {
            if (!IsReady)
            {
                if (_samples > 1)
                {
                    indicesQueue.Enqueue(directionalMovement);
                }
            }

            var smoothedDirectionalIndex =
                ComputeSmoothedDirectionalMovement(indicesQueue,
                                                   directionalMovement,
                                                   ref previousDirectionalIndex);

            return ComputeDirectionalIndex(smoothedDirectionalIndex);
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
        /// Computes the positive directional movement  +DM or +DX.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private double ComputePositiveDirectionalMovement(Candle input)
        {
            var postiveDirectionalMovement = 0.0d;

            if (_previousInput == null) return postiveDirectionalMovement;

            if ((input.High - _previousInput.High) >= (_previousInput.Low - input.Low))
            {
                if ((input.High - _previousInput.High) > 0)
                {
                    postiveDirectionalMovement = input.High - _previousInput.High;
                }
            }

            return postiveDirectionalMovement;
        }

        /// <summary>
        /// Computes the Directional Movement Indicator (DMI period).
        /// </summary>
        /// <returns></returns>
        private double? ComputeDirectionalIndex(double? smoothedDirectionalIndex)
        {
            if (ATRFunction.Value == null || ATRFunction.Value == 0) return 0;

            var directionalIndex = (smoothedDirectionalIndex.Value / ATRFunction.Value) * 100;

            return directionalIndex;
        }

        /// <summary>
        /// Calculates the Smoothed DX
        /// </summary>
        private double? ComputeSmoothedDirectionalMovement(
            Queue<double> indices,
            double directionalMovemenent,
            ref double? previousIndex)
        {
            double? value = null;

            if (_samples == _period + 1)
            {
                value = indices.Average();
                previousIndex = value.Value;
            }
            else if (_samples > _period + 1)
            {
                value = ((previousIndex * (_period - 1)) + directionalMovemenent) / _period;
                previousIndex = value;
            }

            return value;
        }
    }
}
