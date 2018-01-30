using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    /// <summary>
    /// Custom Simple Moving Average (SMA) utilities
    /// </summary>
    public static class SmaUtils
    {
        /// <summary>
        /// Calculate SMA
        /// </summary>
        /// <param name="values">Values that are used to calculate SMA</param>
        /// <param name="period">Period (window) that is used to calculate SMA</param>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>Array with calculated SMA values</returns>
        public static double[] CalculateSma(double[] values, int period, int? decimals = null)
        {
            var result = new double[values.Length];
            var sma = Sma(period);

            for (int i = 0; i < values.Length; i++)
            {
                if (!decimals.HasValue)
                    result[i] = sma(values[i]);
                else
                    result[i] = Math.Round(sma(values[i]), decimals.Value);
            }

            return result;
        }

        private static Func<double, double> Sma(int period)
        {
            var queue = new Queue<double>(period);

            return x =>
            {
                if (queue.Count >= period)
                    queue.Dequeue();

                queue.Enqueue(x);

                return queue.Average();
            };
        }
    }
}
