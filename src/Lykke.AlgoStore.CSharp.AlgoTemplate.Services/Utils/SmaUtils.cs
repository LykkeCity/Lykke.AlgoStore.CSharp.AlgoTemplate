using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public static class SmaUtils
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
