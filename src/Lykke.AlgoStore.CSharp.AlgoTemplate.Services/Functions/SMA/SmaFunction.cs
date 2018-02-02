using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA
{
    /// <summary>
    /// Custom Simple Moving Average (SMA) function
    /// </summary>
    public class SmaFunction
    {
        private Queue<double> _storageQueue;

        /// <summary>
        /// Function capacity (max number of values that will be used for calculus)
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Warm up function with data
        /// </summary>
        /// <param name="values">Values that will be used to warm up data</param>
        /// <param name="capacity">By default function capacity is set to number of provided values. 
        /// If you want to change that you should use this parameter.
        /// If this value is less then number of provided values it will be ignored.</param>
        public void WarmUp(double[] values, int capacity = 0)
        {
            if (values == null)
                throw new ArgumentException();

            Capacity = values.Length < capacity ? values.Length : capacity;

            _storageQueue = Capacity == 0 ? new Queue<double>() : new Queue<double>(Capacity);

            foreach (var value in values)
                AddNewValue(value);
        }

        /// <summary>
        /// Get calculated SMA value
        /// </summary>
        /// <returns>SMA calculated value</returns>
        public double GetSmaValue()
        {
            if (_storageQueue == null || _storageQueue.Count == 0)
                return 0;

            return _storageQueue.Average();
        }

        /// <summary>
        /// Add new value to SMA function
        /// </summary>
        /// <param name="value">New value that will be used in future calculus</param>
        public void AddNewValue(double value)
        {
            if (_storageQueue.Count >= Capacity && Capacity > 0)
                _storageQueue.Dequeue();

            _storageQueue.Enqueue(value);
        }

        /// <summary>
        /// Get value that we are calculating SMA for
        /// </summary>
        /// <returns>Latest value that we calculate SMA for</returns>
        public double GetValue()
        {
            return _storageQueue.Last();
        }
    }
}
