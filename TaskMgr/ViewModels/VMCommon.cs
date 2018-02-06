using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgr.Utils;

namespace TaskMgr.ViewModels
{
    public class VMCommon
    {
        public static SelectList GetStepsLookup(TaskMgrContext context)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            AddEmptyListItem(items);

            foreach (var row in context.Steps.OrderBy(r => r.Name))
            {
                items.Add(new SelectListItem { Text = row.Name, Value = row.StepId.ToString() });
            }
            return new SelectList(items, "Value", "Text");
        }

        public static SelectList GetTasksLookup(TaskMgrContext context)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            AddEmptyListItem(items);

            foreach (var row in context.Tasks.OrderBy(r => r.Name))
            {
                items.Add(new SelectListItem { Text = row.Name, Value = row.TaskId.ToString() });
            }
            return new SelectList(items, "Value", "Text");
        }

        public static SelectList GetSchedulesLookup(TaskMgrContext context)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            AddEmptyListItem(items);

            foreach (var row in context.Schedules.OrderBy(r => r.Name))
            {
                items.Add(new SelectListItem { Text = row.Name, Value = row.ScheduleId.ToString() });
            }
            return new SelectList(items, "Value", "Text");
        }

        public static void AddEmptyListItem(List<SelectListItem> itemsList)
        {
            itemsList.Add(new SelectListItem { Text = "", Value = "" });
        }

        public static SelectList CodeDescDictionaryToSelectList(Dictionary<string, string> dict, bool addEmpty = true, bool sorted = true)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            List<KeyValuePair<string, string>> dictItems = null;

            if (addEmpty)
            {
                AddEmptyListItem(listItems);
            }

            if (sorted)
            {
                dictItems = dict.OrderBy(r => r.Key).ToList();
            }
            else
            {
                dictItems = dict.ToList();
            }

            foreach (var i in dictItems)
            {
                listItems.Add(new SelectListItem { Text = i.Value, Value = i.Key });
            }

            return new SelectList(listItems, "Value", "Text");
        }

        public static SelectList ListToSelectList(List<string> list, bool addEmpty = true, bool sorted = true)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            if (addEmpty)
            {
                AddEmptyListItem(listItems);
            }

            foreach (var str in (sorted ? list : list.OrderBy(o => o).ToList()))
            {
                listItems.Add(new SelectListItem { Text = str, Value = str });
            }

            return new SelectList(listItems, "Value", "Text");
        }
    }
}
