using System;
using System.Collections.Generic;

namespace TaskMgrModels
{
    public partial class Queues
    {
        public Queues()
        {
            QueueSteps = new HashSet<QueueSteps>();
        }

        public int QueueId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime? Completed { get; set; }
        public string Status { get; set; }
        public bool Suspended { get; set; }
        public bool Cancelled { get; set; }

        public Schedules Schedule { get; set; }
        public ICollection<QueueSteps> QueueSteps { get; set; }
    }
}
