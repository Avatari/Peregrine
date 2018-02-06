using System;
using System.Collections.Generic;

namespace TaskMgrModels
{
    public partial class KeyValues
    {
        public string Key { get; set; }
        public DateTime? Dtval { get; set; }
        public int? IntVal { get; set; }
        public string StrVal { get; set; }
        public double? FloatVal { get; set; }
    }
}
