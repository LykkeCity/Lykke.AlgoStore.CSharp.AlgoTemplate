using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class ADX : BaseIndicator
    {
        private int _samples;

        private double? _currentADX { get; set; }

        public override double? Value => _currentADX;

        public int Period { get; }

        #region Additional Parameters

        public override bool IsReady => _samples > Period * 2;

        private Queue<double> _directionalMovementIndices;

        private double? _previousAdx;

        public DMI DMI { get; }
        #endregion

        public ADX(
            int period,
            DateTime startingDate,
            DateTime endingDate,
            CandleTimeInterval candleTimeInterval,
            string assetPair)
            : base(startingDate, endingDate, candleTimeInterval, assetPair)
        {
            Period = period;

            DMI = new DMI(period, startingDate, endingDate, candleTimeInterval, assetPair);

            _directionalMovementIndices = new Queue<double>();
        }

        public override double? WarmUp(IEnumerable<Candle> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
            {
                AddNewValue(value);
            }

            return _currentADX;
        }

        public override double? AddNewValue(Candle value)
        {
            if (value == null)
                throw new ArgumentException();

            _samples++;

            DMI.AddNewValue(value);

            var directionalMovementIndexPlus = DMI.UpwardDirectionalIndex;
            var directionalMovementIndexMinus = DMI.DownwardDirectionalIndex;
            var directionalMovementIndex = 0d;

            if (_samples >= Period + 1)
            {
                if (directionalMovementIndexPlus == 0 && directionalMovementIndexMinus == 0)
                    directionalMovementIndex = 0;
                else
                    directionalMovementIndex = (Math.Abs(directionalMovementIndexPlus.Value - directionalMovementIndexMinus.Value)
                                                    / (directionalMovementIndexPlus.Value + directionalMovementIndexMinus.Value)) * 100;

                _directionalMovementIndices.Enqueue(directionalMovementIndex);
            }

            if (_samples == Period * 2)
            {
                _currentADX = _directionalMovementIndices.Average();
            }
            else if (IsReady)
            {
                _currentADX = ((Period - 1) * _previousAdx + directionalMovementIndex) / Period;
            }
            else
                _currentADX = null;

            _previousAdx = _currentADX;

            return _currentADX;
        }
    }
}
