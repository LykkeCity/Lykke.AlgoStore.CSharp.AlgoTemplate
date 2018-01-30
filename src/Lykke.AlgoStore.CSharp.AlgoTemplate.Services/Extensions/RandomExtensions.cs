using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double minValue, double maxValue, int? decimals = null)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var result = random.NextDouble() * (maxValue - minValue) + minValue;

            if (!decimals.HasValue)
                return result;

            return Math.Round(result, decimals.Value);
        }

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
