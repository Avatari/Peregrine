using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskMgrModels
{
    public partial class Tasks
    {
        public Tasks()
        {
            Schedules = new HashSet<Schedules>();
            TaskSteps = new HashSet<TaskSteps>();
        }

        public int TaskId { get; set; }

        [Required]
        [Display(Name = "Task Name")]
        public string Name { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        [Display(Name = "Step Start Emails")]
        public string StartedEmails { get; set; }

        [Display(Name = "Step Complete Emails")]
        public string CompletedEmails { get; set; }

        [Display(Name = "Failure Emails")]
        public string FailureEmails { get; set; }

        [Display(Name = "Email On Step Start?")]
        public bool EmailsOnStepStart { get; set; }

        [Display(Name = "Email On Step Complete?")]
        public bool EmailsOnStepComplete { get; set; }
        public bool IsValid { get; set; }

        public ICollection<Schedules> Schedules { get; set; }
        public ICollection<TaskSteps> TaskSteps { get; set; }
    }
}
