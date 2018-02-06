
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

namespace Peregrine.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskMgrContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TasksController(TaskMgrContext context, IMapper mapper, IConfiguration Configuration)
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
                var rows = (from r in _context.Tasks
                                select new {r.TaskId, r.Name, ValidStr = r.IsValid.ToYesNo(), EmailsOnStepStartStr = r.EmailsOnStepStart.ToYesNo(),
                                            EmailsOnStepCompleteStr = r.EmailsOnStepComplete.ToYesNo(), Created = r.Created.ToShortDateString()
                                        , Modified = r.Modified.HasValue ? r.Modified.Value.ToShortDateString() : ""});

                //Valid = r.IsValid.HasValue ? (r.IsValid.Value ? "Yes" : "No") : "No"
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    rows = rows.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    rows = rows.Where(m => m.Name.ToLower().Contains(searchValue.ToLower()));
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
                throw;
            }

        }

        public ActionResult AddEdit(int id)
        {
            Tasks row;
            if (id < 0)
            {
                row = new Tasks();
                row.TaskId = -1;
            }
            else
            {
                row = _context.Tasks.Include(r => r.TaskSteps).First(r => r.TaskId == id);
            }

            var vm = _mapper.Map<TasksVM>(row);
            vm.SetStepsLookup(_context);    // set steps lookup of view model

            if (id >= 0)
            {
                vm.TaskStepsList = row.TaskSteps.ToList();
            }

            return PartialView(vm);
        }


        public ActionResult Save(TasksVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Tasks row;
                    if (vm.TaskId < 0)
                    {
                        row = new Tasks();
                    }
                    else
                    {
                        row = _context.Tasks.FirstOrDefault(r => r.TaskId == vm.TaskId);
                    }

                    //_mapper.Map<TasksVM, Tasks>(vm, row);
                    row.Name = vm.Name;
                    row.EmailsOnStepStart = vm.EmailsOnStepStart;
                    row.StartedEmails = vm.StartedEmails;
                    row.EmailsOnStepComplete = vm.EmailsOnStepComplete;
                    row.CompletedEmails = vm.CompletedEmails;

                    if (vm.TaskId < 0)
                    {
                        row.Created = DateTime.Now;
                        _context.Tasks.Add(row);
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
            return View("AddEdit", vm); ;
        }

        public JsonResult IsUnique(string name, int TaskId)
        {
            var isUnique = _context.Tasks.Count(r => r.Name == name && r.TaskId != TaskId) <= 0;
            if (isUnique)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (id >= 0)
                {
                    var row = _context.Tasks.FirstOrDefault(r => r.TaskId == id);

                    if (row != null)
                    {
                        _context.Tasks.Remove(row);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json("Error while delete. Please make sure you delete steps associated with this task first.");
            }
            return Json("Success");
        }


    }
}
