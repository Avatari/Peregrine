using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgr.Utils;
using TaskMgrTypes.Constants;

namespace TaskMgr.ViewModels
{
    public class QueueVM
    {
        public int ScheduleId { get; set; }
        public int TaskId { get; set; }
        public string QueueStatus { get; set; }
        public DateTime? ScheduleStartFrom { get; set; }
        public DateTime? ScheduleStartTo { get; set; }

        public SelectList SchedulesLookupList { get; private set; }
        public SelectList TasksLookupList { get; private set; }
        public SelectList StatusLookupList
        {
            get
            {
                return VMCommon.ListToSelectList(TaskMgrTypes.Constants.QueueStatus.Values);
            }
        }
//        public List<QueueSteps> QueueStepsList { get; set;}

        public void SetLookups(TaskMgrContext context)
        {
            SchedulesLookupList = VMCommon.GetSchedulesLookup(context);
            TasksLookupList = VMCommon.GetTasksLookup(context);
        }
    }

}
