using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX
{
    public class AdxFunction : IFunction
    {
        private readonly int _period;
        private int _samples;
        private Candle _previousInput;
        private AdxParameters _functionParams = new AdxParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;

        #region Additional Parameters

        public bool IsReady => _samples > _period * 2;

        private double? DirectionalMovementIndexMinus { get; set; }
        private double? DirectionalMovementIndexPlus { get; set; }

        private double DirectionalMovementIndex { get; set; }
        private Queue<double> DirectionalMovementIndexes { get; set; }

        private double? PreviousADX { get; set; }
        public double? AverageTrueRange { get; set; }

        private DirectionalMovementIndexPlusFunction DMIPlusFucn { get; set; }
        private DirectionalMovementIndexMinusFunction DMIMinusFucn { get; set; }
        private ATRFunction ATRFunction { get; set; }
        #endregion

        public AdxFunction(AdxParameters adxParameters)
        {
            _period = adxParameters.AdxPeriod;
            _functionParams = adxParameters;

            DirectionalMovementIndexes = _functionParams.AdxPeriod == 0 ? new Queue<double>() : new Queue<double>(_functionParams.AdxPeriod);
            AverageTrueRange = 0.0d;

            DMIPlusFucn = new DirectionalMovementIndexPlusFunction(new DMIParameters() { Period = _period, IsAverageTrueRangeSet = true });
            DMIMinusFucn = new DirectionalMovementIndexMinusFunction(new DMIParameters() { Period = _period, IsAverageTrueRangeSet = true });
            ATRFunction = new ATRFunction(new AtrParameters() { Period = _period });
        }

        public double? WarmUp(IList<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
            {
                _samples++;

                AverageTrueRange = ATRFunction.AddNewValue(value);

                DMIPlusFucn.AverageTrueRange = AverageTrueRange;
                DMIMinusFucn.AverageTrueRange = AverageTrueRange;

                DirectionalMovementIndexPlus = DMIPlusFucn.AddNewValue(value);
                DirectionalMovementIndexMinus = DMIMinusFucn.AddNewValue(value);

                if (_samples >= _period + 1)
                {
                    DirectionalMovementIndex = (Math.Abs(DirectionalMovementIndexPlus.Value - DirectionalMovementIndexMinus.Value)
                                                        / (DirectionalMovementIndexPlus.Value + DirectionalMovementIndexMinus.Value)) * 100;
                    DirectionalMovementIndexes.Enqueue(DirectionalMovementIndex);
                }

                _previousInput = value;

                if (_samples == _period * 2)
                {
                    PreviousADX = DirectionalMovementIndexes.Average();
                }
                else if (IsReady)
                {
                    PreviousADX = ((_period - 1) * PreviousADX + DirectionalMovementIndex) / _period;
                }
                else
                    PreviousADX = null;
            }

            return PreviousADX;
        }

        public double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!IsReady)
                _samples++;

            AverageTrueRange = ATRFunction.AddNewValue(value);

            DMIPlusFucn.AverageTrueRange = AverageTrueRange;
            DMIMinusFucn.AverageTrueRange = AverageTrueRange;

            DirectionalMovementIndexPlus = DMIPlusFucn.AddNewValue(value);
            DirectionalMovementIndexMinus = DMIMinusFucn.AddNewValue(value);

            if (_samples >= _period + 1)
            {
                DirectionalMovementIndex = (Math.Abs(DirectionalMovementIndexPlus.Value - DirectionalMovementIndexMinus.Value)
                                                        / (DirectionalMovementIndexPlus.Value + DirectionalMovementIndexMinus.Value)) * 100;
                DirectionalMovementIndexes.Enqueue(DirectionalMovementIndex);
            }

            _previousInput = value;

            if (_samples == _period * 2)
            {
                PreviousADX = DirectionalMovementIndexes.Average();
                return PreviousADX;
            }
            else if (IsReady)
            {
                PreviousADX = ((_period - 1) * PreviousADX + DirectionalMovementIndex) / _period;
                return PreviousADX;
            }
            else
                return null;
        }

    }
}

