using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class DirectionalMovementIndexMinusFunction : IFunction
    {
        private readonly int _period;
        private readonly bool _isAverageTrueRangeSet = false;

        private int _samples { get; set; }

        private DMIParameters _functionParams = new DMIParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;

        private double? DirectionalMovementIndexMinus { get; set; }
        private double? SmoothedDirectionalMovementMinus { get; set; }
        private double? PreviousSmoothedDirectionalMovementMinus { get; set; }

        private Queue<double> DirectionalMovementMinuses { get; set; }
        private double DirectionalMovementMinus { get; set; }

        /// <summary>
        /// ATR
        /// </summary>
        public double? AverageTrueRange { get; set; }
        public double? PreviousAverageTrueRange { get; set; }

        private Candle _previousInput;
        private ATRFunction ATRFunction { get; set; }

        public DirectionalMovementIndexMinusFunction(DMIParameters dmiParameters)
        {
            _period = dmiParameters.Priod;
            _functionParams = dmiParameters;
            DirectionalMovementMinuses = _functionParams.Priod == 0 ? new Queue<double>() : new Queue<double>(_functionParams.Priod);
            _isAverageTrueRangeSet = _functionParams.IsAverageTrueRangeSet;
            AverageTrueRange = 0.0d;
            ATRFunction = new ATRFunction(new AtrParameters() { Period = _period });
            _samples = 0;
        }

        public double? WarmUp(IList<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
            {
                _samples++;

                if (!_isAverageTrueRangeSet)
                {
                    AverageTrueRange = ATRFunction.AddNewValue(value);
                }

                DirectionalMovementMinus = ComputeNegativeDirectionalMovement(value);

                if (values.IndexOf(value) > 0 && !IsReady)
                {
                    DirectionalMovementMinuses.Enqueue(DirectionalMovementMinus);
                }

                SmoothedDirectionalMovementMinus = ComputeSmoothedDirectionalMovementMinus();
                DirectionalMovementIndexMinus = ComputeNegativeDirectionalIndex();

                _previousInput = value;
            }

            return DirectionalMovementIndexMinus;
        }

        public bool IsReady
        {
            get { return _samples > _period + 1; }
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
            DirectionalMovementIndexMinus = ComputeNegativeDirectionalIndex();

            _previousInput = value;

            return DirectionalMovementIndexMinus;
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
            if (AverageTrueRange == null || AverageTrueRange == 0) return null;

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
