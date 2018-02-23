using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class DirectionalMovemnetIndexPlusFunction : IFunction
    {
        private readonly int _period;
        private DMIParameters _functionParams = new DMIParameters();

        public FunctionParamsBase FunctionParameters => _functionParams;

        private double? DirectionalMovemnetIndexPlus { get; set; }
        private double? SmoothedDirectionalMovementPlus { get; set; }
        private double? PreviousSmoothedDirectionalMovementPlus { get; set; }

        private Queue<double> DirectionalMovementPluses { get; set; }
        private double DirectionalMovementPlus { get; set; }

        /// <summary>
        /// ATR
        /// </summary>
        public double? AverageTrueRange { get; set; }

        private Candle _previousInput;
        private ATRFunction ATRFunction { get; set; }

        private readonly bool _isAverageTrueRangeSet = false;

        public DirectionalMovemnetIndexPlusFunction(DMIParameters dmiParameters)
        {
            _period = dmiParameters.Priod;
            _functionParams = dmiParameters;
            DirectionalMovementPluses = _functionParams.Priod == 0 ? new Queue<double>() : new Queue<double>(_functionParams.Priod);
            _isAverageTrueRangeSet = _functionParams.IsAverageTrueRangeSet;
            AverageTrueRange = 0.0d;
            ATRFunction = new ATRFunction(new AdxParameters() { AdxPriod = _period });
        }

        public double? WarmUp(IList<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
            {
                _functionParams.Samples++;

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
                DirectionalMovemnetIndexPlus = ComputePositiveDirectionalIndex();

                _previousInput = value;
            }

            return DirectionalMovemnetIndexPlus;
        }

        public bool IsReady
        {
            get { return _functionParams.Samples > _period + 1; }
        }

        public double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            _functionParams.Samples++;

            if (!_isAverageTrueRangeSet)
            {
                AverageTrueRange = ATRFunction.AddNewValue(value);
            }

            DirectionalMovementPlus = ComputePositiveDirectionalMovement(value);

            if (!IsReady)
            {
                if (_functionParams.Samples > 1)
                {
                    DirectionalMovementPluses.Enqueue(DirectionalMovementPlus);
                }
            }

            SmoothedDirectionalMovementPlus = ComputeSmoothedDirectionalMovementPlus();
            DirectionalMovemnetIndexPlus = ComputePositiveDirectionalIndex();

            _previousInput = value;

            return DirectionalMovemnetIndexPlus;
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

            if (_functionParams.Samples == _period + 1)
            {
                value = DirectionalMovementPluses.Average();
                PreviousSmoothedDirectionalMovementPlus = value.Value;
            }
            else if (_functionParams.Samples > _period + 1)
            {
                value = ((PreviousSmoothedDirectionalMovementPlus * (_period - 1)) + DirectionalMovementPlus) / _period;
                PreviousSmoothedDirectionalMovementPlus = value;
            }

            return value;
        }
    }
}
