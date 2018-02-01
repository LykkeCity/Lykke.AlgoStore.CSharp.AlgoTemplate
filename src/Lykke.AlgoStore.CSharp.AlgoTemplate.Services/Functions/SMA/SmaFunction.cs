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
        private int _capacity;

        /// <summary>
        /// Warm up function with data
        /// </summary>
        /// <param name="values">Values that will be used to warm up data</param>
        public void WarmUp(double[] values)
        {
            _capacity = values.Length;
            _storageQueue = new Queue<double>(_capacity);

            foreach (var value in values)
                AddNewValue(value);
        }

        /// <summary>
        /// Get calculated SMA value
        /// </summary>
        /// <returns>SMA calculated value</returns>
        public double GetSmaValue() => _storageQueue.Average();

        /// <summary>
        /// Add new value to SMA function
        /// </summary>
        /// <param name="value">New value that will be used in future calculus</param>
        public void AddNewValue(double value)
        {
            if (_storageQueue.Count >= _capacity)
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
