using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrModels;

namespace TaskMgr.ViewModels
{
    public class StepsVM : Steps
    {
        public SelectList AvailableClasses { get; set; }
    }
}
