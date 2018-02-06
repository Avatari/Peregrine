using System;
using System.Collections.Generic;

namespace TaskMgrModels
{
    public partial class QueueSteps
    {
        public int QueueStepId { get; set; }
        public int QueueId { get; set; }
        public int Seq { get; set; }
        public int? StepId { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? ExecutionStarted { get; set; }
        public DateTime? ExecutionCompleted { get; set; }
        public string ReturnValues { get; set; }
        public string FailureInfo { get; set; }
        public string Status { get; set; }
        public DateTime? LastExecutionSuspended { get; set; }
        public string PostExecutionDecision { get; set; }

        public Queues Queue { get; set; }
        public Steps Step { get; set; }
    }
}
