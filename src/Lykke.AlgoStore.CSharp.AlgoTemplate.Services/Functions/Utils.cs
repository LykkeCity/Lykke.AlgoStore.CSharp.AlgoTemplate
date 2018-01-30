using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions
{
    public static class Utils
    {
        public static double[] CalculateSma(double[] values, int period)
        {
            var result = new double[values.Length];
            var sma = Sma(period);

            for (int i = 0; i < values.Length; i++)
                result[i] = sma(values[i]);

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
