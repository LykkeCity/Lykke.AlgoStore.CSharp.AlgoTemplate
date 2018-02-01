using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="System.Random"/>
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Get random double number
        /// </summary>
        /// <param name="random"><see cref="System.Random"/></param>
        /// <param name="minValue">Minimal random generated value</param>
        /// <param name="maxValue">Maximal random generated value</param>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>Random double value that is between minValue and maxValue</returns>
        public static double NextDouble(this Random random, double minValue, double maxValue, int? decimals = null)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var result = random.NextDouble() * (maxValue - minValue) + minValue;

            if (!decimals.HasValue)
                return result;

            return Math.Round(result, decimals.Value);
        }

        /// <summary>
        /// Generate list of random double values
        /// </summary>
        /// <param name="random"><see cref="System.Random"/></param>
        /// <param name="capacity">Number of values to generate</param>
        /// <param name="minValue">Minimal random generated value</param>
        /// <param name="maxValue">Maximal random generated value</param>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>List with random double values that are between minValue and maxValue</returns>
        public static List<double> GenerateRandomDoubles(this Random random, int capacity, double minValue, double maxValue, int? decimals = null)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException();

            var result = new List<double>();

            for (int i = 0; i < capacity; i++)
            {
                result.Add(random.NextDouble(minValue, maxValue, decimals));
            }

            return result;
        }
    }
}
