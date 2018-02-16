using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.EMA
{
    public class EmaFunction : AbstractFunction
    {
        private readonly double _k;
        private readonly int _period;

        private Queue<double> _storageQueue;
        private EmaParameters _functionParams = new EmaParameters();
        private double? _emaPreviousPeriod;

        private bool isReady = false;

        public override FunctionParamsBase FunctionParameters => _functionParams;

        public EmaFunction(EmaParameters emaParameters)
        {
            _period = emaParameters.EmaPeriod;
            _k = GetWeightingMultiplier(emaParameters.EmaPeriod);
            _functionParams = emaParameters;
        }


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
        public override double? WarmUp(double[] values)
        {
            if (values == null)
                throw new ArgumentException();

            _storageQueue = _functionParams.EmaPeriod == 0 ? new Queue<double>() : new Queue<double>(_functionParams.EmaPeriod);

            foreach (var value in values)
                _emaPreviousPeriod = GetInitialValues(value);

            return _emaPreviousPeriod;
        }

        /// <summary>
        /// Add new value to EMA function
        /// </summary>
        /// <param name="value">New value that will be used in future calculus</param>
        public override double? AddNewValue(double value)
        {
            if (!isReady)
            {
                _emaPreviousPeriod = GetInitialValues(value);
                return _emaPreviousPeriod;
            }

            return GetEmaValue(value);
        }

        public double? GetEmaAndAddValue(double value)
        {
            return AddNewValue(value);
        }

        /// <summary>
        /// Gets if fuction is ready for using aftere warmup values
        /// </summary>
        public bool IsReady()
        {
            return isReady;
        }

        private double? GetInitialValues(double value)
        {
            if (_storageQueue.Count >= _period && _period > 0)
                _storageQueue.Dequeue();

            _storageQueue.Enqueue(value);

            if (_storageQueue.Count < _period)
                return null;

            isReady = true;
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
