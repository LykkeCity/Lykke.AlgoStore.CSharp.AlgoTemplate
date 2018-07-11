using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    /// <summary>
    /// Custom Simple Moving Average (SMA) function
    /// </summary>
    public class SMA : AbstractIndicator
    {
        private readonly Queue<double> _storageQueue = new Queue<double>();

        public int Capacity { get; set; }

        public override double? Value => GetSmaValue();
        public override bool IsReady => Value != null;

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
            if (_storageQueue.Count >= Capacity && Capacity > 0)
                _storageQueue.Dequeue();

            _storageQueue.Enqueue(value);

            return GetSmaValue();
        }
    }
}
