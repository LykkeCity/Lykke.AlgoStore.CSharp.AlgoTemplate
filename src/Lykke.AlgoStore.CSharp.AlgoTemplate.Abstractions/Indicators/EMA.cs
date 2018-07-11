using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    public class EMA : AbstractIndicator
    {
        private double _k;
        private int _period;

        private Queue<double> _storageQueue;
        private double? _emaPreviousPeriod;

        private double? _value;

        private bool _isReady = false;

        public int Period
        {
            get => _period;
            set
            {
                _period = value;
                _k = GetWeightingMultiplier(_period);
            }
        }

        public override double? Value => _value;
        public override bool IsReady => _isReady;

        /// <summary>Calculates weighting multiplier for an EmaFunction
        /// </summary>
        /// <param name="period">The period of the EMA</param>
        /// <returns>The  weighting multiplier</returns>
        public static double GetWeightingMultiplier(int period)
        {
            return 2.0d / (period + 1.0d);
        }

        /// <summary>
        /// Warm up function with data
        /// </summary>
        /// <param name="values">Values that will be used to warm up data</param>
        public override double? WarmUp(IEnumerable<double> values)
        {
            if (values == null)
                throw new ArgumentException();

            _storageQueue = Period == 0 ? new Queue<double>() : new Queue<double>(Period);

            foreach (var value in values)
                _emaPreviousPeriod = GetInitialValue(value);

            _value = _emaPreviousPeriod;
            return _emaPreviousPeriod;
        }

        /// <summary>
        /// Add new value to EMA function
        /// </summary>
        /// <param name="value">New value that will be used in future calculus</param>
        public override double? AddNewValue(double value)
        {
            if (!_isReady)
            {
                _emaPreviousPeriod = GetInitialValue(value);
                return _emaPreviousPeriod;
            }

            _value = GetEmaValue(value);
            return _value;
        }

        public double? GetEmaAndAddValue(double value)
        {
            return AddNewValue(value);
        }

        private double? GetInitialValue(double value)
        {
            if (_storageQueue.Count >= _period && _period > 0)
                _storageQueue.Dequeue();

            _storageQueue.Enqueue(value);

            if (_storageQueue.Count < _period)
                return null;

            _isReady = true;
            return _storageQueue.Average();
        }

        private double? GetEmaValue(double value)
        {
            if (!_emaPreviousPeriod.HasValue)
                return null;

            double emaValue = (value - _emaPreviousPeriod.Value) * _k + _emaPreviousPeriod.Value;
            _emaPreviousPeriod = emaValue;
            return emaValue;
        }
    }
}
