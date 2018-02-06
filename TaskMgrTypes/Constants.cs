using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMgrTypes.Constants
{
    
    public class PostExecutionDecision
    {
        // this can be customized

        public const string Yes = "Yes";
        public const string No = "No";
        public const string Ok = "Ok";
        public const string Cancel = "Cancel";
        public const string Error= "Error";
        public const string NoError = "No Error";
        public const string CustomA = "CustomA";

        public static readonly List<string> Values = new List<string>() { Yes, No, Ok, Cancel, Error, NoError, CustomA };
    }





    public class Frequency
    {
        public const string RepeatDailyCode = "DAY";
        public const string RepeatAfterXMinutesCode = "XMIN";
        public const string DayOfWeekCode = "DOW";
        public const string DayOfMonthCode = "DOM";
        public const string FirstDayOfMonthCode = "FDOM";
        public const string FirstBusinessDayOfMonthCode = "FBDOM";
        public const string LastDayOfMonthCode = "LDOM";
        public const string LastBusinessDayOfMonthCode = "LBDOM";
        public const string OneTimeCode = "ONCE";

        public const string RepeatDailyDescription = "Daily";
        public const string RepeatAfterXMinutesDescription = "Repeat After X Minutes";
        public const string DayOfWeekDescription = "Day of Week";
        public const string DayOfMonthDescription = "Day of Month";
        public const string FirstDayOfMonthDescription = "First Day of Month";
        public const string FirstBusinessDayOfMonthDescription = "First Business Day of Month";
        public const string LastDayOfMonthDescription = "Last Day of Month";
        public const string LastBusinessDayOfMonthDescription = "Last Business Day of Month";
        public const string OneTimeDescription = "One Time";

        public static readonly Dictionary<string, string> Values = new Dictionary<string, string>() {
                                                                                                                {RepeatDailyCode, RepeatDailyDescription },
                                                                                                                {RepeatAfterXMinutesCode, RepeatAfterXMinutesDescription }
                                                                                                                , { DayOfWeekCode, DayOfWeekDescription }
                                                                                                                , { DayOfMonthCode, DayOfMonthDescription }
                                                                                                                , { FirstDayOfMonthCode,  FirstDayOfMonthDescription }
                                                                                                                , { FirstBusinessDayOfMonthCode,  FirstBusinessDayOfMonthDescription }
                                                                                                                , { LastDayOfMonthCode ,  LastDayOfMonthDescription }
                                                                                                                , { LastBusinessDayOfMonthCode,  LastBusinessDayOfMonthDescription }
                                                                                                                , { OneTimeCode, OneTimeDescription }

                                                                                                               };
    }

    public class QueueStatus
    {
        public const string Added = "Added";
        public const string Started = "Started";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";     // can be completed, suspended or cancelled

        public static readonly List<string> Values = new List<string>() { Added , Started, InProgress, Completed};

    }


    public class QueueStepStatus
    {
        public const string Added = "Added";
        public const string InProgress = "In Progress";
        public const string Idled = "Idled";
        public const string Completed = "Completed";
    }



    public class ConfigKey
    {
        public const string ConnectionString = "TaskMgrConnectionString";
        public const string DllWithPath = "DynamicStepClassesDllWithFullPath";
        public const string ScheduleIntervalInSeconds = "ScheduleIntervalInSeconds";

        // email setting keys constatnts
        public const string SmtpServer = "SmtpServer";
        public const string SmtpPortNumber = "SmtpPortNumber";
        public const string FromEmailId = "FromEmailId";
        public const string FromEmailTitle = "FromEmailTitle";
        public const string TaskMgrInfoFile = "TaskMgrInfoFile";
        
    }

    public class NonCatogarized
    {

        public const string QueueStepPK = "QueueStepId";

        public const string Yes = "Yes";
        public const string No = "No";

    }

    public class KeyValuesTableKey
    {
        public const string LastHeartBeat = "LastHeartBeat";
    }
}
