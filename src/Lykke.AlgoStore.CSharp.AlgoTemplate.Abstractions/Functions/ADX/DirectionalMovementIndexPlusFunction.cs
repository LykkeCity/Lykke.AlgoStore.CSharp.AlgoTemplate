﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.ADX
{
    public class DirectionalMovementIndexPlusFunction : IFunction
    {
        private readonly int _period;
        private readonly bool _isAverageTrueRangeSet = false;
        private int _samples { get; set; }

        private double? _currentDMIPlusValue { get; set; }
        private double? SmoothedDirectionalMovementPlus { get; set; }
        private double? PreviousSmoothedDirectionalMovementPlus { get; set; }

        private Queue<double> DirectionalMovementPluses { get; set; }
        private double DirectionalMovementPlus { get; set; }

        public bool IsReady => _samples > _period + 1;

        public double? Value => _currentDMIPlusValue;

        public DMIParameters _functionParams = new DMIParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;

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

                DirectionalMovementPlus = ComputePositiveDirectionalMovement(value);

                if (!isFirstElement && !IsReady)
                {
                    DirectionalMovementPluses.Enqueue(DirectionalMovementPlus);
                }

                isFirstElement = false;

                SmoothedDirectionalMovementPlus = ComputeSmoothedDirectionalMovementPlus();
                _currentDMIPlusValue = ComputePositiveDirectionalIndex();

                _previousInput = value;
            }

            return _currentDMIPlusValue;
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
            _currentDMIPlusValue = ComputePositiveDirectionalIndex();

            _previousInput = value;

            return _currentDMIPlusValue;
        }

        /// <summary>
        /// Computes the Plus Directional Movment Indicator (+DMI period).
        /// </summary>
        /// <returns></returns>
        private double? ComputePositiveDirectionalIndex()
        {
            if (AverageTrueRange == null || AverageTrueRange == 0) return 0;

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
