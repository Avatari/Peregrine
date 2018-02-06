
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
    public class StepsController : Controller
    {
        private readonly TaskMgrContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public StepsController(TaskMgrContext context, IMapper mapper, IConfiguration Configuration)
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
                var rows = (from r in _context.Steps
                                select new {r.StepId, r.Name, r.Class, Created = r.Created.ToShortDateString()
                                        , Modified = r.Modified.HasValue ? r.Modified.Value.ToShortDateString() : ""});

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    rows = rows.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    rows = rows.Where(m => m.Name.ToLower().Contains(searchValue.ToLower()) 
                                || m.Class.ToLower().Contains(searchValue.ToLower()));
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

        public ActionResult AddEdit(int id)
        {
            Steps steps;
            if (id < 0)
            {
                steps = new Steps();
                steps.StepId = -1;
            }
            else
            {
                steps = _context.Steps.First(r => r.StepId == id);
            }
            var vm = _mapper.Map<StepsVM>(steps);

            DynamicStepsUtil loader = new DynamicStepsUtil();
            var lst = loader.GetStepsClasses(_configuration[ConfigKey.DllWithPath]);
            lst.Insert(0, "");

            var selectList = new List<SelectListItem>();
            foreach (var item in lst)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }

            vm.AvailableClasses = new SelectList(selectList, "Value", "Text");

            return PartialView(vm);
        }

        public ActionResult Save(StepsVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Steps steps;
                    if (vm.StepId < 0)
                    {
                        steps = new Steps();
                    }
                    else
                    {
                        steps = _context.Steps.FirstOrDefault(r => r.StepId == vm.StepId);
                    }

                    //_mapper.Map<StepsVM, Steps>(vm, steps);
                    steps.Name = vm.Name;
                    steps.Class = vm.Class;



                    if (vm.StepId < 0)
                    {
                        steps.Created = DateTime.Now;
                        _context.Steps.Add(steps);
                    }
                    else
                    {
                        steps.Modified = DateTime.Now;
                    }
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return View();
                }
                return Json("Information saved");
            }
            return View();

        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (id >= 0)
                {
                    var steps = _context.Steps.FirstOrDefault(r => r.StepId == id);

                    if (steps != null)
                    {
                        _context.Steps.Remove(steps);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message);
            }
            return Json("Deleted");
        }


    }
}
