
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
    public class TaskStepsController : Controller
    {
        private readonly TaskMgrContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TaskStepsController(TaskMgrContext context, IMapper mapper, IConfiguration Configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = Configuration;
        }

        public IActionResult Index(int id)
        {
            TaskStepsVM vm = new TaskStepsVM(_context);
            vm.TaskId = id;

            var task = _context.Tasks.First(r => r.TaskId == id);
            vm.SetTaskProperties(id);
            return View(vm);
        }

        public IActionResult LoadData(int id)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
 
                var rows = (from t in _context.TaskSteps
                            join s in _context.Steps on t.StepId equals s.StepId into ts
                            from s in ts.DefaultIfEmpty()
                            where t.TaskId == id
                                select new {t.TaskStepId, t.TaskId, t.StepId, t.Seq, Step = (s == null ? "" : s.Name)
                                , On = t.PostExecutionDecision, t.GotoSeq}
                            );

  
                //Paging 
                var data = rows.OrderBy(r => r.Seq).ToList();
                //Returning Json Data
                return Json(new { draw = draw, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }

        public ActionResult AddEdit(int id, int taskId)
        {
            TaskSteps row;
            if (id < 0)
            {
                row = new TaskSteps();
                row.TaskStepId = -1;
                row.TaskId = taskId;
            }
            else
            {
                row = _context.TaskSteps.First(r => r.TaskStepId == id);
            }

            var vm = _mapper.Map<TaskStepsVM>(row);

            vm.SetStepsLookup(_context);    // set steps lookup of view model
            vm.SetStepSequence(_context);

            return PartialView(vm);
        }

 
        public ActionResult Save(TaskStepsVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TaskSteps row;
                    if (vm.TaskStepId < 0)
                    {
                        row = new TaskSteps();
                        row.TaskId = vm.TaskId;
                        vm.SetStepSequence(_context);       // not sure why vm does bind Seq from view so re generate
                        row.Seq = vm.Seq;
                    }
                    else
                    {
                        row = _context.TaskSteps.FirstOrDefault(r => r.TaskStepId == vm.TaskStepId);
                    }

                    //_mapper.Map<TaskStepsVM, TaskSteps>(vm, row);

                    row.StepId = vm.StepId;
                    row.PostExecutionDecision = vm.PostExecutionDecision;
                    row.GotoSeq = vm.GotoSeq;

                    var task = _context.Tasks.First(r => r.TaskId == vm.TaskId);

                    // run task validation and update status
                    //task.IsValid = TasksVM.ValidateTask(vm.TaskId);

                    if (vm.TaskStepId < 0)
                    {
                        _context.TaskSteps.Add(row);
                    }
                    else
                    {
                        task.Modified = DateTime.Now;
                    }

                    _context.SaveChanges();


                    return RedirectToAction("Index", new { id = vm.TaskId });
                }
                catch (Exception ex)
                {
                    vm.SetStepsLookup(_context);    // set steps lookup of view model
                    return View("AddEdit", vm);
                }
            }
            vm.SetStepsLookup(_context);    // set steps lookup of view model
            return View("AddEdit", vm); ;
        }


        public IActionResult MoveUp(int id)
        {
            // get task id from step id
            var step1 = _context.TaskSteps.First(r => r.TaskStepId == id);
            int taskId = step1.TaskId;

            var step2 = _context.TaskSteps.Where(r => r.TaskId == taskId && r.Seq < step1.Seq).OrderByDescending(r => r.Seq).FirstOrDefault();

            if (step2 != null)
            {
                // swap seq and save steps
                int seq = step1.Seq;
                step1.Seq = step2.Seq;
                step2.Seq = seq;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { id = taskId });
        }

        public IActionResult MoveDown(int id)
        {
            // get task id from step id
            var step1 = _context.TaskSteps.First(r => r.TaskStepId == id);
            int taskId = step1.TaskId;

            var step2 = _context.TaskSteps.Where(r => r.TaskId == taskId && r.Seq > step1.Seq).OrderBy(r => r.Seq).FirstOrDefault();

            if (step2 != null)
            {
                // swap seq and save steps
                int seq = step1.Seq;
                step1.Seq = step2.Seq;
                step2.Seq = seq;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { id = taskId });
        }

        public ActionResult Delete(int id)
        {
            int taskId = 0;
            try
            {
                if (id >= 0)
                {
                    var taskStep = _context.TaskSteps.FirstOrDefault(r => r.TaskStepId == id);
                    taskId = taskStep.TaskId;
                    if (taskStep != null)
                    {
                        _context.TaskSteps.Remove(taskStep);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index", new { id = taskId });
        }
    }
}
