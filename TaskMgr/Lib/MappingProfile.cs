using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgr.ViewModels;

namespace TaskMgr.Lib
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Steps, StepsVM>();
            CreateMap<StepsVM, Steps>();

            CreateMap<Tasks, TasksVM>();
            CreateMap<TasksVM, Tasks>();

            CreateMap<TaskSteps, TaskStepsVM>();
            CreateMap<TaskStepsVM, TaskSteps>();

            CreateMap<Schedules, SchedulesVM>();
            CreateMap<SchedulesVM, Schedules>();

        }
    }
}
