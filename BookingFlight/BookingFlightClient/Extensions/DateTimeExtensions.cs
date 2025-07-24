using System;

namespace BookingFlightClient.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts DateOnly to DateTime by combining with current time
        /// </summary>
        public static DateTime ToDateTimeWithCurrentTime(this DateOnly dateOnly)
        {
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            return dateOnly.ToDateTime(currentTime);
        }

        /// <summary>
        /// Converts nullable DateOnly to DateTime by combining with current time
        /// </summary>
        public static DateTime ToDateTimeWithCurrentTime(this DateOnly? dateOnly)
        {
            if (!dateOnly.HasValue)
                return DateTime.Now;
                
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            return dateOnly.Value.ToDateTime(currentTime);
        }

        /// <summary>
        /// Converts DateOnly to DateTime with specified time
        /// </summary>
        public static DateTime ToDateTime(this DateOnly dateOnly, int hour, int minute, int second = 0)
        {
            var timeOnly = new TimeOnly(hour, minute, second);
            return dateOnly.ToDateTime(timeOnly);
        }
    }
}
