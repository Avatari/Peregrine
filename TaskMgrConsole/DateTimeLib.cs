using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMgrConsole
{
    public class DateTimeLib
    {
        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return date.AddDays(-1 * (date.Day - 1));
        }

        public static DateTime FirstDayOfNextMonth(DateTime date)
        {
            return FirstDayOfMonth(date).AddMonths(1);
        }

        public static DateTime LastDayOfCurrentMonth(DateTime date)
        {
            return FirstDayOfMonth(date).AddMonths(1).AddDays(-1);
        }

        public static DateTime LastDayOfNextMonth(DateTime date)
        {
            return FirstDayOfMonth(date).AddMonths(2).AddDays(-1);
        }

        // function does not consider holidays
        public static DateTime FirstBusinessDayOfMonth(DateTime date)
        {
            DateTime retVal = FirstDayOfMonth(date);

            while (retVal.DayOfWeek == DayOfWeek.Saturday || retVal.DayOfWeek == DayOfWeek.Sunday)
            {
                retVal = retVal.AddDays(1);
            }

            return retVal;
        }

        // function does not consider holidays
        public static DateTime FirstBusinessDayOfNextMonth(DateTime date)
        {
            DateTime retVal = FirstDayOfNextMonth(date);

            while(retVal.DayOfWeek == DayOfWeek.Saturday || retVal.DayOfWeek == DayOfWeek.Sunday)
            {
                retVal = retVal.AddDays(1);
            }

            return retVal;
        }

        // function does not consider holidays
        public static DateTime LastBusinessDayOfMonth(DateTime date)
        {
            DateTime retVal = FirstDayOfNextMonth(date).AddDays(-1);

            while (retVal.DayOfWeek == DayOfWeek.Saturday || retVal.DayOfWeek == DayOfWeek.Sunday)
            {
                retVal = retVal.AddDays(-1);
            }

            return retVal;
        }

        // function does not consider holidays
        public static DateTime LastBusinessDayOfNextMonth(DateTime date)
        {
            DateTime retVal = LastDayOfNextMonth(date);

            while (retVal.DayOfWeek == DayOfWeek.Saturday || retVal.DayOfWeek == DayOfWeek.Sunday)
            {
                retVal = retVal.AddDays(-1);
            }

            return retVal;
        }

        public static DateTime NextMonthDay(DateTime date, int day)
        {
            return MonthDay(FirstDayOfNextMonth(date), day);
        }

        // function adds a month to given day of the month date
        // exact same day of month is not guaranteed (e.g. Feb does not have 30 days so will return 28/29 depending days in month
        public static DateTime MonthDay(DateTime date, int day)
        {
 
            int daysInNextMonth = FirstDayOfNextMonth(date).AddDays(-1).Day;

            DateTime retVal;
            if (day <= daysInNextMonth)
            {
                retVal = FirstDayOfMonth(date).AddDays(day - 1);
            }
            else
            {
                // return last day of next month
                retVal = FirstDayOfMonth(date).AddDays(daysInNextMonth - 1);
            }

            return retVal;
        }

        // get corrected start date based on week day
        public static DateTime WeekDay(DateTime startDT, int value)
        {
            DateTime retVal = startDT;

            for(int i = 0; i < 7; i++)
            {
                if ((int)retVal.DayOfWeek + 1 == value)
                {
                    break;
                }
                retVal = retVal.AddDays(1);
            }
            return retVal;
        }
    }
}
