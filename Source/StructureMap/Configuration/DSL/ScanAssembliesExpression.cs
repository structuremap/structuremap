using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class ScanAssembliesExpression : IExpression
    {
        private List<AssemblyGraph> _assemblies = new List<AssemblyGraph>();

        public void Configure(PluginGraph graph)
        {
            foreach (AssemblyGraph assembly in _assemblies)
            {
                graph.Assemblies.Add(assembly);
            }
        }

        public IExpression[] ChildExpressions
        {
            get { return new IExpression[0]; }
        }

        public ScanAssembliesExpression IncludeTheCallingAssembly()
        {
            Assembly callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                _assemblies.Add(new AssemblyGraph(callingAssembly));
            }

            return this;
        }

        private static Assembly findTheCallingAssembly()
        {
            StackTrace trace = new StackTrace(Thread.CurrentThread, false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public ScanAssembliesExpression IncludeAssemblyContainingType<T>()
        {
            _assemblies.Add(AssemblyGraph.ContainingType<T>());

            return this;
        }
    }
}
