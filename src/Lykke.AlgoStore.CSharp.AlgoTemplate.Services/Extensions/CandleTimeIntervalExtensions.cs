using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class CandleTimeIntervalExtensions
    {
        /// <summary>
        /// Increments a <see cref="DateTime"/> using a given <see cref="CandleTimeInterval"/>
        /// </summary>
        /// <param name="interval">The <see cref="CandleTimeInterval"/></param>
        /// <param name="timestamp">The <see cref="DateTime"/> to increment</param>
        /// <returns>A new <see cref="DateTime"/> containing the incremented time</returns>
        public static DateTime IncrementTimestamp(this CandleTimeInterval interval, DateTime timestamp)
        {
            if (interval == CandleTimeInterval.Month) // Month can't be accurate because of the different month lenghts, so add it manually instead
                return timestamp.AddMonths(1);
            else if (interval == CandleTimeInterval.Hour4) // Hour4 is defined as 7200 seconds, which is 2 hours, so add 4 hours manually instead
                return timestamp.AddHours(4);
            else
                return timestamp.AddSeconds((int)interval);
        }

        /// <summary>
        /// Decrements a <see cref="DateTime"/> using a given <see cref="CandleTimeInterval"/>
        /// </summary>
        /// <param name="interval">The <see cref="CandleTimeInterval"/></param>
        /// <param name="timestamp">The <see cref="DateTime"/> to decrement</param>
        /// <returns>A new <see cref="DateTime"/> containing the decremented time</returns>
        public static DateTime DecrementTimestamp(this CandleTimeInterval interval, DateTime timestamp)
        {
            if (interval == CandleTimeInterval.Month) // Month can't be accurate because of the different month lenghts, so add it manually instead
                return timestamp.AddMonths(-1);
            else if (interval == CandleTimeInterval.Hour4) // Hour4 is defined as 7200 seconds, which is 2 hours, so add 4 hours manually instead
                return timestamp.AddHours(-4);
            else
                return timestamp.AddSeconds(-(int)interval);
        }
    }
}
