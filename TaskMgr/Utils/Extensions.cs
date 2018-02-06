using TaskMgrTypes.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrTypes.Constants;

namespace TaskMgr.Utils
{
    public static class Extensions
    {
        public static String ToYesNo(this bool value)
        {
            return value ? NonCatogarized.Yes : NonCatogarized.No;
        }
    }
}
