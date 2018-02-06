using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskMgrModels
{
    public partial class Schedules
    {
        public Schedules()
        {
            Queues = new HashSet<Queues>();
        }

        public int ScheduleId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Schedule Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Task")]
        public int TaskId { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        public string Freq { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Modified { get; set; }

        [Display(Name = "Is Disabled?")]
        public bool Disabled { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? NextQueue { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDt { get; set; }

        [Display(Name = "Frequency Identifier")]
        public int? PeriodInterval { get; set; }


        public bool IsCompleted { get; set; }

        public Tasks Task { get; set; }
        public ICollection<Queues> Queues { get; set; }
    }
}
