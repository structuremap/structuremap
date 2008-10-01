using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression that directs StructureMap to scan the named assemblies
    /// for [PluginFamily] and [Plugin] attributes
    /// </summary>
    public class ScanAssembliesExpression
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly Registry _registry;

        public ScanAssembliesExpression(Registry registry)
        {
            _registry = registry;
            _registry.addExpression(graph =>
            {
                foreach (Assembly assembly in _assemblies)
                {
                    graph.Assemblies.Add(assembly);
                }
            });
        }

        public ScanAssembliesExpression IncludeTheCallingAssembly()
        {
            Assembly callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
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
            _assemblies.Add(typeof (T).Assembly);

            return this;
        }

        public ScanAssembliesExpression IncludeAssemblyContainingType(Type type)
        {
            _assemblies.Add(type.Assembly);

            return this;
        }

        public ScanAssembliesExpression AddAllTypesOf<PLUGINTYPE>()
        {
            return AddAllTypesOf(typeof (PLUGINTYPE));
        }

        public ScanAssembliesExpression AddAllTypesOf(Type pluginType)
        {
            _registry.addExpression(pluginGraph =>
            {
                pluginGraph.Assemblies.AddScanner(new FindAllTypesFilter(pluginType));
            });

            return this;
        }

        public ScanAssembliesExpression IncludeAssembly(string assemblyName)
        {
            Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
            _assemblies.Add(assembly);

            return this;
        }

        public ScanAssembliesExpression With(ITypeScanner scanner)
        {
            _registry.addExpression(graph => graph.Assemblies.AddScanner(scanner));


            return this;
        }

        public ScanAssembliesExpression With<T>() where T : ITypeScanner, new()
        {
            _registry.addExpression(graph => graph.Assemblies.AddScanner<T>());
            return this;
        }

        // TODO: Need a test here
        public ScanAssembliesExpression With<T>(Action<T> configure) where T : ITypeScanner, new()
        {
            T scanner = new T();
            configure(scanner);

            _registry.addExpression(graph => graph.Assemblies.AddScanner(scanner));
            return this;
        }
    }
}