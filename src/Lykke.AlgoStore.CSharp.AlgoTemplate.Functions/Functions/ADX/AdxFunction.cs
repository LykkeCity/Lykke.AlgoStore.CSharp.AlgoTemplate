using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Functions.ADX
{
    public class AdxFunction : IFunction
    {

        private readonly DirectionalMovementIndexPlusFunction _dmiPlusFucn;
        private readonly DirectionalMovementIndexMinusFunction _dmiMinusFucn;
        private readonly ATRFunction _atrFunction;

        private readonly int _period;
        private int _samples;
        private AdxParameters _functionParams = new AdxParameters();
        public FunctionParamsBase FunctionParameters => _functionParams;
        private double? _currentADX { get; set; }

        public double? Value => _currentADX;

        #region Additional Parameters

        public bool IsReady => _samples > _period * 2;

        private double? DirectionalMovementIndexMinus { get; set; }
        private double? DirectionalMovementIndexPlus { get; set; }

        private double DirectionalMovementIndex { get; set; }
        private Queue<double> DirectionalMovementIndexes { get; set; }

        private double? PreviousADX { get; set; }
        private double? AverageTrueRange { get; set; }

        public DirectionalMovementIndexPlusFunction DMIPlusFucn => _dmiPlusFucn;
        public DirectionalMovementIndexMinusFunction DMIMinusFucn => _dmiMinusFucn;
        public ATRFunction ATRFunction => _atrFunction;
        #endregion

        public AdxFunction(AdxParameters adxParameters)
        {
            _period = adxParameters.AdxPeriod;
            _functionParams = adxParameters;

            DirectionalMovementIndexes = _functionParams.AdxPeriod == 0 ? new Queue<double>() : new Queue<double>(_functionParams.AdxPeriod);
            AverageTrueRange = 0.0d;

            _dmiPlusFucn = new DirectionalMovementIndexPlusFunction(new DMIParameters() { Period = _period, IsAverageTrueRangeSet = true });
            _dmiMinusFucn = new DirectionalMovementIndexMinusFunction(new DMIParameters() { Period = _period, IsAverageTrueRangeSet = true });
            _atrFunction = new ATRFunction(new AtrParameters() { Period = _period });
        }

        public double? WarmUp(IEnumerable<Candle> values)
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
                    if (DirectionalMovementIndexPlus == 0 && DirectionalMovementIndexMinus == 0)
                        DirectionalMovementIndex = 0;
                    else
                        DirectionalMovementIndex = (Math.Abs(DirectionalMovementIndexPlus.Value - DirectionalMovementIndexMinus.Value)
                                                        / (DirectionalMovementIndexPlus.Value + DirectionalMovementIndexMinus.Value)) * 100;

                    DirectionalMovementIndexes.Enqueue(DirectionalMovementIndex);
                }

                if (_samples == _period * 2)
                {
                    _currentADX = DirectionalMovementIndexes.Average();
                }
                else if (IsReady)
                {
                    _currentADX = ((_period - 1) * PreviousADX + DirectionalMovementIndex) / _period;
                }
                else
                    _currentADX = null;

                PreviousADX = _currentADX;
            }

            return _currentADX;
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

            if (_samples == _period * 2)
            {
                _currentADX = DirectionalMovementIndexes.Average();
                PreviousADX = _currentADX;
            }
            else if (IsReady)
            {
                _currentADX = ((_period - 1) * PreviousADX + DirectionalMovementIndex) / _period;
            }
            else
                _currentADX = null;

            PreviousADX = _currentADX;
            return _currentADX;
        }
    }
}

