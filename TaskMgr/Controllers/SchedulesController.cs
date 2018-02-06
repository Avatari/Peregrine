
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskMgrModels;
using System.Collections.ObjectModel;
using AutoMapper;
using TaskMgr.ViewModels;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Configuration;
using TaskMgr.Utils;
using TaskMgrTypes.Constants;

namespace Peregrine.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly TaskMgrContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SchedulesController(TaskMgrContext context, IMapper mapper, IConfiguration Configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadData()
        {
            try
            {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            // Skiping number of Rows count
            var start = Request.Form["start"].FirstOrDefault();
            // Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();
            // Sort Column Name
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            // Sort Column Direction ( asc ,desc)
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            // Search Value from (Search box)
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10,20,50,100)
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

                // Getting all Customer data
                var rows = (from s in _context.Schedules
                            join t in _context.Tasks on s.TaskId equals t.TaskId
                            select new
                            {
                                s.ScheduleId,
                                s.Name,
                                TaskName = t.Name,
                                Freq = (string.IsNullOrEmpty(s.Freq) ? "" : Frequency.Values[s.Freq]),
                                Identifier = (!s.PeriodInterval.HasValue ? "" : s.PeriodInterval.Value.ToString()),
                                Start = Convert.ToString(s.Start),
                                End = (s.EndDt.HasValue ? Convert.ToString(s.EndDt) : ""),
                                Disabled = s.Disabled.ToYesNo(),
                                Completed = s.IsCompleted ? "1" : "0"
                            });

            //Sorting
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                rows = rows.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                rows = rows.Where(m => m.Name.ToLower().Contains(searchValue.ToLower())
                            || m.Name.ToLower().Contains(searchValue.ToLower()));
            }

            //total number of rows count 
            recordsTotal = rows.Count();
            //Paging 
            var data = rows.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }

        public ActionResult AddEdit(int id)
        {
            Schedules row;
            if (id < 0)
            {
                row = new Schedules();
                row.ScheduleId = -1;
                row.Start = DateTime.Now;
            }
            else
            {
                row = _context.Schedules.First(r => r.ScheduleId == id);
            }

            var vm = _mapper.Map<SchedulesVM>(row);
            vm.SetTasksLookup(_context);    // set steps lookup of view model


            return PartialView(vm);
        }


        public ActionResult Save(SchedulesVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Schedules row;
                    if (vm.ScheduleId < 0)
                    {
                        row = new Schedules();
                    }
                    else
                    {
                        row = _context.Schedules.FirstOrDefault(r => r.ScheduleId == vm.ScheduleId);
                    }

                    row.Name = vm.Name;
                    row.TaskId = vm.TaskId;
                    row.Freq = vm.Freq;
                    row.PeriodInterval = vm.PeriodInterval;
                    row.Start = vm.Start;
                    row.EndDt = vm.EndDt;
                    row.Disabled = vm.Disabled;

                    if (vm.ScheduleId < 0)
                    {
                        row.Created = DateTime.Now;
                        _context.Schedules.Add(row);
                    }
                    else
                    {
                        row.Modified = DateTime.Now;
                    }

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    return View("AddEdit", vm);
                }
            }
            vm.SetTasksLookup(_context);
            return View("AddEdit", vm);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (id >= 0)
                {
                    var row = _context.Schedules.FirstOrDefault(r => r.ScheduleId == id);

                    if (row != null)
                    {
                        _context.Schedules.Remove(row);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json("Error while deleting schedule. Please make sure to delete related data before deleting schedule.");
            }
            return Json("Success");
        }


    }
}
