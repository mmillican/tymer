using System;
using System.Globalization;

namespace tymer
{
    public static class Extensions
    {
        public static (DateTime Start, DateTime End) GetWeekStartAndEnd(this DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            var start = date.AddDays(-1 * diff).Date;
            var end = start.AddDays(6);

            return (Start: start.Date, End: end.Date);
        }
    }
}
