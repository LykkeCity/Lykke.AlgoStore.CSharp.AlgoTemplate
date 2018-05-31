using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDefaultDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString(Constants.DateTimeFormat);
        }
    }
}
