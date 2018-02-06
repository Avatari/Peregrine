using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskMgrModels
{
    public partial class TaskSteps
    {
        public int TaskStepId { get; set; }
        public int TaskId { get; set; }

        [Display(Name = "Sequence")]
        public int Seq { get; set; }

        [Display(Name = "Step")]
        public int? StepId { get; set; }

        [Display(Name = "When Execution")]
        public string PostExecutionDecision { get; set; }

        [Display(Name = "Go to Sequence")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int? GotoSeq { get; set; }

        public Steps Step { get; set; }
        public Tasks Task { get; set; }
    }
}
