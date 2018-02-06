using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskMgrModels
{
    public partial class Steps
    {
        public Steps()
        {
            QueueSteps = new HashSet<QueueSteps>();
            TaskSteps = new HashSet<TaskSteps>();
        }

        public int StepId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Step Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Class To Execute")]
        public string Class { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public ICollection<QueueSteps> QueueSteps { get; set; }
        public ICollection<TaskSteps> TaskSteps { get; set; }
    }
}
