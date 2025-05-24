using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Helpers
{
    public static class DateUtils
    {
        public static DateTime GetStartOfWeek(DateTime dt)
        {
            var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.Date.AddDays(-1 * diff);
        }
        public static string FormatDuration(double durationSeconds)
        {
            var duration = TimeSpan.FromSeconds(durationSeconds);

            if (duration.TotalHours >= 1)
            {
                if (duration.Minutes == 0)
                    return $"{(int)duration.TotalHours}h";
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";
            }
            else
            {
                if (duration.Minutes > 0)
                    return $"{duration.Minutes}m {duration.Seconds}s";
                return $"{duration.Seconds}s";
            }
        }
    }
}
