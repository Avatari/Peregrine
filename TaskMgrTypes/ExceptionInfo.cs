using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMgrTypes
{
    public class ExceptionInfo
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string AdditionalInfo { get; set; }

        public override string ToString()
        {
            return "Error ::: " + (string.IsNullOrEmpty(Code) ? "" : " Code : " + Code) 
                    + " Message : " + Message 
                    + (string.IsNullOrEmpty(AdditionalInfo) ? "" : " Additional Information : " + AdditionalInfo);
        }
    }
}
