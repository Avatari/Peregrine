using FluentValidation;
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
    public class TasksVM : Tasks
    {
        //public string Valid
        //{
        //    get
        //    {
        //        return IsValid.HasValue ? (IsValid.Value ? "Yes" : "No") : "No";
        //    }
        //}

        public List<TaskSteps> TaskStepsList { get; set; }
        public SelectList OnExecutionLookupList { get; set; }
        public SelectList StepsLookupList { get; set; }

        public TasksVM()
        {
            TaskStepsList = new List<TaskSteps>();

            OnExecutionLookupList = VMCommon.ListToSelectList(PostExecutionDecision.Values, true);
        }

        public void SetStepsLookup(TaskMgrContext context)
        {
            StepsLookupList = VMCommon.GetStepsLookup(context);
        }
    }

    public class TasksValidator : AbstractValidator<TasksVM>
    {
        private TaskMgrContext _context;
        public TasksValidator(TaskMgrContext context) : this()
        {
            _context = context;
        }
        public TasksValidator()
        {
            RuleFor(x => x.Name).Must(UniqueName).WithMessage("Task Name must be unique.");
            RuleFor(x => x.StartedEmails).Must((vmInstance, emails) => ValidateEmails(vmInstance.EmailsOnStepStart, emails)).WithMessage("Invalid Email addresses.");
            RuleFor(x => x.CompletedEmails).Must((vmInstance, emails) => ValidateEmails(vmInstance.EmailsOnStepComplete, emails)).WithMessage("Invalid Email addresses.");
        }

        private bool ValidateEmails(bool isSendEmails, string emails)
        {
            bool allValid = true;

            if (isSendEmails)
            {
                if (string.IsNullOrWhiteSpace(emails))
                {
                    allValid = false;
                }
                else
                {
                    EmailAddressAttribute _emailAddressAttribute = new EmailAddressAttribute();
                    var emailsArr = emails.ToString().Split(new[] { ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    allValid = emailsArr.All(addr => _emailAddressAttribute.IsValid(addr));
                }
            }
            return (allValid);
        }

        private bool UniqueName(TasksVM instance, string name)
        {
            var matchRec = _context.Tasks
                                .Where(x => x.Name.Trim() == name.Trim() && x.TaskId != instance.TaskId)
                                .SingleOrDefault();

            return matchRec == null;
        }

    }

}
