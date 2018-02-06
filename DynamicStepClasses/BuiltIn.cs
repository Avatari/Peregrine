using System;
using System.Collections.Generic;
using System.Text;
using TaskMgrTypes;
using TaskMgrTypes.Constants;

namespace DynamicStepClasses.BuiltIn
{

    // always Yes
    public class AlwaysYes : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();
            output.PostExecutionDecision = PostExecutionDecision.Yes;

            return output;
        }
    }

    // always No
    public class AlwaysNo : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();
            output.PostExecutionDecision = PostExecutionDecision.No;

            return output;
        }
    }

    // place holder, doing nothing
    public class EndPlaceHolder : IStep
    {
        public OutputValues Execute(InputValues input)
        {
            OutputValues output = new OutputValues();

            return output;
        }
    }

}
