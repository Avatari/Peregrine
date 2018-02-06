using System;

namespace TaskMgrTypes
{
    public interface IStep
    {
        OutputValues Execute(InputValues inputValues);
    }

    public class InputValues
    {
        // Queue Step Id just in case step code need more details from TaskMgr db
        public int QueueStepId { get; set; }

        // queue start date time, if step code wants to make decisions depending queue start date time
        public DateTime ScheduledStart { get; set; }

        // can be converted to specific type using JsonConvert.DeserializeObject<T>(Values) by specific Step code
        public string Values { get; set; }
    }

    public class OutputValues
    {
        // Should return one of these codes: StepOnSuccessCode/StepOnFailureCode/StepIdleCode
        public string PostExecutionDecision { get; set; }

        public bool IsIdled { get; set; }

        // Step code should set JsonConvert.SerializeObject<T>(Values)
        public string Values { get; set; }

        // set optional failure details when step is failed or being Idle
        public FailureInfo FailureInfo { get; set; }
    }

    public class FailureInfo
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
