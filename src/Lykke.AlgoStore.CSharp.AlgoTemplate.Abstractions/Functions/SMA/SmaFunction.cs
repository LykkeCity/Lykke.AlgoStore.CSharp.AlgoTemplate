using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA
{
    /// <summary>
    /// Custom Simple Moving Average (SMA) function
    /// </summary>
    public class SmaFunction : AbstractFunction
    {
        private Queue<double> _storageQueue;
        private SmaParameters _functionParams = new SmaParameters();

        public override FunctionParamsBase FunctionParameters => _functionParams;
        public override double? Value => GetSmaValue();
        public override bool IsReady => Value != null;

        /// <summary>
        /// Initializes new instance of <see cref="SmaFunction"/>
        /// </summary>
        /// <param name="capacity">The capacity of the function. 
        /// (NOTE: this will eventualy be part of the <see cref="FunctionParamsBase"/>)</param>
        public SmaFunction(SmaParameters smaParameters)
        {
            _functionParams = smaParameters;
            _storageQueue = _functionParams.Capacity == 0 ? new Queue<double>() : new Queue<double>(_functionParams.Capacity);
        }

        /// <summary>
        /// Warm up function with data
        /// </summary>
        /// <param name="values">Values that will be used to warm up data</param>
        public override double? WarmUp(IEnumerable<double> values)
        {
            if (values == null)
                throw new ArgumentException();

            foreach (var value in values)
                AddNewValue(value);

            return GetSmaValue();
        }

        /// <summary>
        /// Get calculated SMA value
        /// </summary>
        /// <returns>SMA calculated value</returns>
        public double? GetSmaValue()
        {
            if (_storageQueue == null || _storageQueue.Count == 0)
                return null;

            return _storageQueue.Average();
        }

        /// <summary>
        /// Add new value to SMA function
        /// </summary>
        /// <param name="value">New value that will be used in future calculus</param>
        public override double? AddNewValue(double value)
        {
            if (_storageQueue.Count >= _functionParams.Capacity && _functionParams.Capacity > 0)
                _storageQueue.Dequeue();

            _storageQueue.Enqueue(value);

            return GetSmaValue();
        }
    }
}
