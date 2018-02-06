using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgr.Utils;
using TaskMgrTypes.Constants;

namespace TaskMgr.ViewModels
{
    public class SchedulesVM : Schedules
    {
        public SelectList FreqLookupList { get; set; }
        public SelectList TasksLookupList { get; private set; }

        public SchedulesVM()
        {
            FreqLookupList = VMCommon.CodeDescDictionaryToSelectList(Frequency.Values);
        }

        public void SetTasksLookup(TaskMgrContext context)
        {
            TasksLookupList = VMCommon.GetTasksLookup(context);
        }

    }

    public class SchedulesValidator : AbstractValidator<SchedulesVM>
    {
        private TaskMgrContext _context;
        public SchedulesValidator(TaskMgrContext context) : this()
        {
            _context = context;
        }

        public SchedulesValidator()
        {
            RuleFor(x => x.Name).Must(UniqueName).WithMessage("Schedule Name must be unique.");
            RuleFor(x => x.PeriodInterval).Must((vmInstance, periodInterval, context) => ValidatePeriodInterval(vmInstance.Freq, periodInterval, context)).WithMessage("{ErrorMessage}");
            RuleFor(x => x.EndDt).Must((vmInstance, endDt) => BeAValidNullableDate(endDt)).WithMessage("Can be empty or a valid DateTime.");
            RuleFor(x => x.Start).Must((vmInstance, start) => BeAValidDate(start)).WithMessage("Must be a valid DateTime.");

        }

        private bool BeAValidDate(DateTime date)
        {
            return date != DateTime.MinValue;
        }

        private bool BeAValidNullableDate(DateTime? date)
        {
            return date.HasValue || date != DateTime.MinValue;
        }

        private bool UniqueName(SchedulesVM instance, string name)
        {
            var matchRec = _context.Schedules
                                .Where(x => x.Name.Trim() == name.Trim() && x.ScheduleId != instance.ScheduleId)
                                .SingleOrDefault();

            return matchRec == null;
        }

        private bool ValidatePeriodInterval(string freq, int? periodInterval, PropertyValidatorContext context)
        {
            bool isValid = true;
            string error = "";
            switch (freq)
            {
                case Frequency.RepeatAfterXMinutesCode:
                    isValid = periodInterval.HasValue && periodInterval.Value >= 15;
                    if (!isValid)
                    {
                        error = "Interval must be provided and at least 15 Minutes.";
                    }
                    break;
                case Frequency.DayOfWeekCode:
                    isValid = periodInterval.HasValue && periodInterval.Value >= 1 && periodInterval.Value <= 7;
                    if (!isValid)
                    {
                        error = "Day of week must be between 1 (Sunday) and 7 (Saturday).";
                    }
                    break;
                case Frequency.DayOfMonthCode:
                    isValid = periodInterval.HasValue && periodInterval.Value >= 1 && periodInterval.Value <= 31;
                    if (!isValid)
                    {
                        error = "Day of week must be between 1 and 31.";
                    }
                    break;
            }

            context.MessageFormatter.AppendArgument("ErrorMessage", error);
            return isValid;

        }
    }
}
