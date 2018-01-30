using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions
{
    public static class Utils
    {
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

        public static double NextDouble(this Random random, double minValue, double maxValue, int? decimals = null)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var result = random.NextDouble() * (maxValue - minValue) + minValue;

            if (!decimals.HasValue)
                return result;

            return Math.Round(result, decimals.Value);
        }

        public static List<double> GenerateRandomDoubles(int capacity, double minValue, double maxValue, int? decimals = null)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException();

            var randomGenerator = new Random();
            var result = new List<double>();

            for (int i = 0; i < capacity; i++)
            {
                result.Add(randomGenerator.NextDouble(minValue, maxValue, decimals));
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
