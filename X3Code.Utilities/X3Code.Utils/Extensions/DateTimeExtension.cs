using System;
using System.Data.SqlTypes;

namespace X3Code.Utils.Extensions
{
    /// <summary>
    /// DateTime calculation and useful stuff to work with
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Get's the SQM Min Date
        /// </summary>
        public static DateTime SqlMinDate => SqlDateTime.MinValue.Value;
        /// <summary>
        /// Get's the SQM Max Date
        /// </summary>
        public static DateTime SqlMaxDate => SqlDateTime.MaxValue.Value;

        /// <summary>
        /// Checks if the given DateTime object is within the MS SQL DateTime-Range.
        /// If the Date is out of this range, it returns the SQL-Min Date
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ToSqlMinDateTimeIfOutOfRange(this DateTime source)
        {
            if (source.IsInSqlDateTimeRange())
            {
                return source;
            }
            return SqlMinDate;
        }

        /// <summary>
        /// Returns true, if the given DateTime is within the SQL Timerange
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInSqlDateTimeRange(this DateTime source)
        {
            return !IsBelowSqlMinDate(source) && !IsHigherSqlMaxDate(source);
        }

        /// <summary>
        /// Return true, if the given date is smaller (more in the past) than SQL.MinDate
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsBelowSqlMinDate(this DateTime source)
        {
            return source < SqlMinDate;
        }

        /// <summary>
        /// Return false, if the given date is bigger (more in the future) than SQL.MaxDate
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsHigherSqlMaxDate(this DateTime source)
        {
            return source > SqlMaxDate;
        }

        /// <summary>
        /// Sets the clock of a DateTime object to 00:00.00
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ClockTo00(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, 0, 0, 0);
        }

        /// <summary>
        /// Sets the clock of a DateTime object to 23:59.59
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ClockTo24(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, 23, 59, 59);
        }

        #region Weekdays

        /// <summary>
        /// Gets the next monday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextMonday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(-1);
        }

        /// <summary>
        /// Gets the next tuesday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextTuesday(this DateTime source)
        {
            //http://stackoverflow.com/questions/6346119/datetime-get-next-tuesday
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)source.DayOfWeek + 7) % 7;
            return source.AddDays(daysUntilTuesday);
        }

        /// <summary>
        /// Gets the next wednesday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextWednesday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(1);
        }

        /// <summary>
        /// Gets the next thursday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextThursday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(2);
        }

        /// <summary>
        /// Gets the next friday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextFriday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(3);
        }

        /// <summary>
        /// Gets the next saturday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextSaturday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(4);
        }

        /// <summary>
        /// Gets the next sunday from the given DateTime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetNextSunday(this DateTime source)
        {
            return source.GetNextTuesday().AddDays(5);
        }

        #endregion

        /// <summary>
        /// Adds seven days to the datetime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetSevenDaysLater(this DateTime source)
        {
            return source.AddDays(7);
        }

        /// <summary>
        /// Adds fourteen days to the datetime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetFourteenDaysLater(this DateTime source)
        {
            return source.AddDays(14);
        }

        /// <summary>
        /// Substracts seven days to the datetime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetSevenDaysEalier(this DateTime source)
        {
            return source.AddDays(-7);
        }

        /// <summary>
        /// Substracts fourteen days to the datetime object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetFourteenDaysEarlier(this DateTime source)
        {
            return source.AddDays(-14);
        }
    }
}
