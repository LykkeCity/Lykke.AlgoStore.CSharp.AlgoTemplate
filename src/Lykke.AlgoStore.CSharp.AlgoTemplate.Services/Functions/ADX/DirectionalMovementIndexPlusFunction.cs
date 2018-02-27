using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class DirectionalMovementIndexPlusFunction : IFunction
    {
        private readonly int _period;
        private readonly bool _isAverageTrueRangeSet = false;
        private int _samples { get; set; }

        private DMIParameters _functionParams = new DMIParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;

        private double? DirectionalMovementIndexPlus { get; set; }
        private double? SmoothedDirectionalMovementPlus { get; set; }
        private double? PreviousSmoothedDirectionalMovementPlus { get; set; }

        private Queue<double> DirectionalMovementPluses { get; set; }
        private double DirectionalMovementPlus { get; set; }

        public bool IsReady => _samples > _period + 1;

        /// <summary>
        /// ATR
        /// </summary>
        public double? AverageTrueRange { get; set; }

        private Candle _previousInput;
        private ATRFunction ATRFunction { get; set; }

        public DirectionalMovementIndexPlusFunction(DMIParameters dmiParameters)
        {
            _period = dmiParameters.Period;
            _functionParams = dmiParameters;
            DirectionalMovementPluses = _functionParams.Period == 0 ? new Queue<double>() : new Queue<double>(_functionParams.Period);
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

                DirectionalMovementPlus = ComputePositiveDirectionalMovement(value);

                if (values.IndexOf(value) > 0 && !IsReady)
                {
                    DirectionalMovementPluses.Enqueue(DirectionalMovementPlus);
                }

                SmoothedDirectionalMovementPlus = ComputeSmoothedDirectionalMovementPlus();
                DirectionalMovementIndexPlus = ComputePositiveDirectionalIndex();

                _previousInput = value;
            }

            return DirectionalMovementIndexPlus;
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

            DirectionalMovementPlus = ComputePositiveDirectionalMovement(value);

            if (!IsReady)
            {
                if (_samples > 1)
                {
                    DirectionalMovementPluses.Enqueue(DirectionalMovementPlus);
                }
            }

            SmoothedDirectionalMovementPlus = ComputeSmoothedDirectionalMovementPlus();
            DirectionalMovementIndexPlus = ComputePositiveDirectionalIndex();

            _previousInput = value;

            return DirectionalMovementIndexPlus;
        }

        /// <summary>
        /// Computes the Plus Directional Movment Indicator (+DMI period).
        /// </summary>
        /// <returns></returns>
        private double? ComputePositiveDirectionalIndex()
        {
            if (AverageTrueRange == null || AverageTrueRange == 0) return null;

            var positiveDirectionalIndex = (SmoothedDirectionalMovementPlus.Value / AverageTrueRange.Value) * 100;

            return positiveDirectionalIndex;
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
        /// Calculates the Smoothed +DX
        /// </summary>
        /// <returns></returns>
        private double? ComputeSmoothedDirectionalMovementPlus()
        {
            double? value = null;

            if (_samples == _period + 1)
            {
                value = DirectionalMovementPluses.Average();
                PreviousSmoothedDirectionalMovementPlus = value.Value;
            }
            else if (IsReady)
            {
                value = ((PreviousSmoothedDirectionalMovementPlus * (_period - 1)) + DirectionalMovementPlus) / _period;
                PreviousSmoothedDirectionalMovementPlus = value;
            }

            return value;
        }
    }
}
