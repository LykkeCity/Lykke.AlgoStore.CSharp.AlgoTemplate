using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class ADX : IIndicator
    {
        private DMIPlus _dmiPlusFucn;
        private DMIMinus _dmiMinusFucn;
        private ATR _atrFunction;

        private int _period;
        private int _samples;

        private double? _currentADX { get; set; }

        public double? Value => _currentADX;

        public int Period
        {
            get => _period;
            set
            {
                _period = value;
                InitIndicators(value);
            }
        }

        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public CandleTimeInterval CandleTimeInterval { get; set; }
        public string AssetPair { get; set; }

        #region Additional Parameters

        public bool IsReady => _samples > _period * 2;

        private double? DirectionalMovementIndexMinus { get; set; }
        private double? DirectionalMovementIndexPlus { get; set; }

        private double DirectionalMovementIndex { get; set; }
        private Queue<double> DirectionalMovementIndexes { get; set; }

        private double? PreviousADX { get; set; }
        private double? AverageTrueRange { get; set; }

        public DMIPlus DMIPlusFucn => _dmiPlusFucn;
        public DMIMinus DMIMinusFucn => _dmiMinusFucn;
        public ATR ATRFunction => _atrFunction;
        #endregion

        public ADX()
        {
            DirectionalMovementIndexes = new Queue<double>();
            AverageTrueRange = 0.0d;
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

        private void InitIndicators(int period)
        {
            if (_dmiPlusFucn != null)
                throw new InvalidOperationException("Cannot set period more than once");

            _dmiPlusFucn = new DMIPlus(period);
            _dmiMinusFucn = new DMIMinus(period);
            _atrFunction = new ATR { Period = period };
        }
    }
}

