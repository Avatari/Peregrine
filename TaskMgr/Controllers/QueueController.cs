
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
using DynamicStepsLib;
using TaskMgrTypes.Constants;

namespace Peregrine.Controllers
{
    public class QueueController : Controller
    {
        private readonly TaskMgrContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public QueueController(TaskMgrContext context, IMapper mapper, IConfiguration Configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            QueueVM returnVM = new QueueVM();
            returnVM.SetLookups(_context);

            return View(returnVM);
        }

        public IActionResult LoadData(QueueVM vm)
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


                //    var rows = (from q in _context.Queues
                //                    join qs in _context.QueueSteps on q.QueueId equals qs.QueueId into QSTP
                //                    join s in _context.Schedules on q.ScheduleId equals s.ScheduleId into SCH
                //                from qstp in QSTP.DefaultIfEmpty()
                //                    join stp in _context.Steps on qstp.StepId equals stp.StepId into STP
                //                from stp in STP.DefaultIfEmpty()
                //                from sch in SCH
                //                    join t in _context.Tasks on sch.TaskId equals t.TaskId into TSK
                //                from tsk in TSK

                //                select new
                //                {
                //                    ScheduleName = sch.Name,
                //                    TaskName = tsk.Name,
                //                    q.ScheduledStart,
                //                    q.Completed,
                //                    q.Status,
                //                    q.Suspended,
                //                    q.Cancelled,
                //                    Seq = (qstp == null ? 0 : qstp.Seq ),
                //                    StepName = (stp == null ? "" : stp.Name)
                //                }
                //);

                //DateTime? scheduledFrom = null;
                //DateTime? scheduledTo = null;

                //if (vm.ScheduleStartFrom != null && vm.ScheduleStartFrom.HasValue)
                //{
                //    scheduledFrom = vm.ScheduleStartFrom.Value;
                //}

                //if (vm.ScheduleStartTo != null && vm.ScheduleStartTo.HasValue)
                //{
                //    scheduledTo = vm.ScheduleStartTo.Value;
                //}

                //if (scheduledFrom != null && scheduledTo != null)
                //{
                //    DateTime temp = scheduledFrom.Value;
                //    scheduledFrom = scheduledTo;
                //    scheduledTo = temp;
                //}
                //&& (scheduledFrom == null ? true : q.ScheduledStart >= scheduledFrom)
                //&& (scheduledTo == null ? true : q.ScheduledStart <= scheduledTo)


                var rows = (from q in _context.Queues
                            join s in _context.Schedules on q.ScheduleId equals s.ScheduleId into SCH
                            from sch in SCH
                            join t in _context.Tasks on sch.TaskId equals t.TaskId into TSK
                            from tsk in TSK
                            where (vm.ScheduleId <= 0 ? true : sch.ScheduleId == vm.ScheduleId)
                                && (vm.TaskId <= 0 ? true : tsk.TaskId == vm.TaskId)
                            select new
                            {
                                q.QueueId,
                                ScheduleName = sch.Name,
                                TaskName = tsk.Name,
                                ScheduledStart = Convert.ToString(q.ScheduledStart),
                                Completed = Convert.ToString(q.Completed),
                                q.Status,
                                Suspended = q.Suspended.ToYesNo(),
                                Cancelled = q.Cancelled.ToYesNo(),
                            });

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    rows = rows.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    rows = rows.Where(m => m.ScheduleName.ToLower().Contains(searchValue.ToLower()) 
                                || m.TaskName.ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count 
                recordsTotal = rows.Count();
                //Paging 
                var data = rows.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IActionResult QueueSteps(int id)
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

                var rows = _context.QueueSteps.Include(r => r.Step).Where(r => r.QueueId == id).OrderBy(r => r.Seq)
                            .Select(r => new {r.QueueStepId, r.Seq, StepName = r.Step.Name, r.Status, Added = Convert.ToString(r.Added),
                                    ExecutionStated = Convert.ToString(r.ExecutionStarted),
                                    ExecutionCompleted = Convert.ToString(r.ExecutionCompleted), r.ReturnValues, r.FailureInfo,
                                    LastExecutionSuspended = Convert.ToString(r.LastExecutionSuspended), r.PostExecutionDecision});

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    rows = rows.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //total number of rows count 
                recordsTotal = rows.Count();

                //no Paging, take all
                var data = rows.ToList();

                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult ToggleCancel(int id)
        {
            try
            {
                if (id >= 0)
                {
                    var row = _context.Queues.FirstOrDefault(r => r.QueueId == id);

                    if (row != null)
                    {
                        row.Cancelled = !row.Cancelled;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json("Error while cancel: " + ex.Message);
            }
            return Json("Success");
        }

        public ActionResult ToggleSuspend(int id)
        {
            try
            {
                if (id >= 0)
                {
                    var row = _context.Queues.FirstOrDefault(r => r.QueueId == id);

                    if (row != null)
                    {
                        row.Suspended = !row.Suspended;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json("Error while cancel: " + ex.Message);
            }
            return Json("Success");
        }

    }
}
