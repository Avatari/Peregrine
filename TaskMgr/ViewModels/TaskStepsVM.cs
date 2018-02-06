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
    public class TaskStepsVM : TaskSteps
    {
        public SelectList OnExecutionLookupList { get; set; }
        public SelectList StepsLookupList { get; private set; }

        public string TaskName { get; set; }
        public bool IsTaskValid { get; set; }
        public string TaskValidationError { get; set; }

        private TaskMgrContext _context;

        public TaskStepsVM(TaskMgrContext context) : this()
        {
            _context = context;
            
        }

        public TaskStepsVM()
        {
            OnExecutionLookupList = VMCommon.ListToSelectList(TaskMgrTypes.Constants.PostExecutionDecision.Values, true);
        }

        public void SetStepsLookup(TaskMgrContext context)
        {
            StepsLookupList = VMCommon.GetStepsLookup(context);
        }

        public void SetStepSequence(TaskMgrContext context)
        {
            if (TaskStepId < 0)
            {
                Seq = (context.TaskSteps.Where(r => r.TaskId == TaskId).Count() + 1) * 10;
            }
        }

        public void SetTaskProperties(int id)
        {
            TaskName = _context.Tasks.First(r => r.TaskId == id).Name;

            string error = "";
            IsTaskValid = ValidateTaskSteps(id, out error);
            TaskValidationError = error;
        }

        public bool ValidateTaskSteps(int id, out string error)
        {
            bool isValid = true;
            error = "";

            var task = _context.Tasks.First(r => r.TaskId == id);
            var steps = _context.TaskSteps.Where(r => r.TaskId == id).OrderBy(r => r.Seq).ToList();

            if (steps.Count <= 0)
            {
                isValid = false;
                error = "Task should have at least one step.";
            }
            else
            {
                // check gotos are valid and forward only
                foreach (var st in steps)
                {
                    if (!string.IsNullOrEmpty(st.PostExecutionDecision) && !st.GotoSeq.HasValue)
                    {
                        isValid = false;
                        error = "Sequence " + st.Seq.ToString() + " Goto is required when 'When Execution' is selected.";
                        break;
                    }

                    if (st.GotoSeq.HasValue)
                    {
                        if (st.GotoSeq.Value <= st.Seq)
                        {
                            isValid = false;
                            error = "Sequence " + st.Seq.ToString() + " Goto " + st.GotoSeq.Value.ToString() + " is invalid, it must be forward only.";
                            break;
                        }

                        if (!steps.Any(r => r.Seq == st.GotoSeq.Value))
                        {
                            isValid = false;
                            error = "Sequence " + st.Seq.ToString() + " Goto " + st.GotoSeq.Value.ToString() + " step does not exist.";
                            break;
                        }
                    }
                }
            }

            if (task.IsValid != isValid)
            {
                // update valid flag
                task.IsValid = isValid;
                _context.SaveChanges();
            }

            return isValid;
        }

    }

    public class TaskStepsValidator : AbstractValidator<TaskStepsVM>
    {
        private TaskMgrContext _context;
        public TaskStepsValidator(TaskMgrContext context) : this()
        {
            _context = context;
        }
        public TaskStepsValidator()
        {
            //RuleFor(x => x.Seq).Must((vmInstance, seq) => ValidateSeq(vmInstance.TaskId, vmInstance.TaskStepId, seq)).WithMessage("Must not be any existing sequence number, and must be less then the last sequence number.");
            RuleFor(x => x.PostExecutionDecision).Must((vmInstance, PostExecutionDecision) => ValidatePostExecutionDecision(vmInstance.TaskId, vmInstance.Seq, vmInstance.PostExecutionDecision, vmInstance.GotoSeq)).WithMessage("Goto Sequence is not provided.");
            RuleFor(x => x.GotoSeq).Must((vmInstance, gotoSeq) => ValidateGotoSeq(vmInstance.TaskId, vmInstance.Seq, gotoSeq)).WithMessage("Must be forward going sequence number.");
        }

        private bool ValidatePostExecutionDecision(int taskId, int seq, string postExecutionDecision, int? gotoSeq)
        {
            bool allValid = string.IsNullOrEmpty(postExecutionDecision) || gotoSeq.HasValue;

            return allValid;
        }

        private bool ValidateGotoSeq(int taskId, int seq, int? gotoSeq)
        {
            bool allValid = true;

            if (gotoSeq.HasValue && gotoSeq.Value > 0)
            {
                allValid = (gotoSeq.Value % 10 == 0 && gotoSeq.Value > seq);
            }
            return (allValid);
        }
    }


}
