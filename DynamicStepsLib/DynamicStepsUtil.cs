using TaskMgrTypes;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace DynamicStepsLib
{
    public class DynamicStepsUtil
    {
        public List<String> GetStepsClasses(string dllWithPath)
        {
            var asl = new AssemblyLoader();
            var asm = asl.LoadFromAssemblyPath(dllWithPath);

            List<String> stepClasses = new List<String>();
            foreach (var t in asm.GetTypes())
            {
                if (t.GetInterfaces().Any(o => o == typeof(IStep)))
                {
                    stepClasses.Add(t.ToString());
                }
            }
            // test first class instantiation
            //dynamic obj0 = Activator.CreateInstance(stepClasses[0], new object[] { "My Input" });
            //object retVal = obj0.Execute();


            //var myType = asm.GetType("Step1");
            //var myInstance = Activator.CreateInstance(myType);
            return stepClasses;
        }

        // test execute given step, will change later with better inputs and outputs including catching and returning exceptions
        public  OutputValues ExecuteStep(string dllWithPath, string step, InputValues inputValues )
        {
            var asl = new AssemblyLoader();
            var asm = asl.LoadFromAssemblyPath(dllWithPath);
            var stepClass = asm.GetType(step);
            // test first class instantiation
            dynamic execObj = Activator.CreateInstance(stepClass);
            OutputValues retVal = execObj.Execute(inputValues);
            return retVal;
        }

    }

    public class AssemblyLoader : AssemblyLoadContext
    {
        // Not exactly sure about this
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            var assembly = Assembly.Load(new AssemblyName(res.First().Name));
            return assembly;
        }
    }
}
