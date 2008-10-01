using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using StructureMap.Diagnostics;

namespace StructureMap.Graph
{
    public class AssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<ITypeScanner> _scanners = new List<ITypeScanner>();

        public AssemblyScanner()
        {
            With<FamilyAttributeScanner>();
            With<PluggableAttributeScanner>();
            With<FindRegistriesScanner>();
        }

        public int Count
        {
            get { return _assemblies.Count; }
        }

        public void ScanForAll(PluginGraph pluginGraph)
        {
            _assemblies.ForEach(assem => scanTypesInAssembly(assem, pluginGraph));
        }

        private void scanTypesInAssembly(Assembly assembly, PluginGraph graph)
        {
            Type[] exportedTypes;
            try
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    _scanners.ForEach(scanner => scanner.Process(type, graph));
                }
            }
            catch (Exception ex)
            {
                graph.Log.RegisterError(170, ex, assembly.FullName);
            }
        }

        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }

        public void Assembly(string assemblyName)
        {
            Assembly(AppDomain.CurrentDomain.Load(assemblyName));
        }

        public bool Contains(string assemblyName)
        {
            foreach (Assembly assembly in _assemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return true;
                }
            }

            return false;
        }

        public void With(ITypeScanner scanner)
        {
            if (_scanners.Contains(scanner)) return;

            _scanners.Add(scanner);
        }

        public void With<T>() where T : ITypeScanner, new()
        {
            ITypeScanner previous = _scanners.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
            {
                With(new T());
            }
        }

        public void IgnoreRegistries()
        {
            _scanners.RemoveAll(x => x is FindRegistriesScanner);
        }

        public void TheCallingAssembly()
        {
            Assembly callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        private static Assembly findTheCallingAssembly()
        {
            StackTrace trace = new StackTrace(Thread.CurrentThread, false);

            Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
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

        public void AssemblyContainingType<T>()
        {
            _assemblies.Add(typeof(T).Assembly);
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.Assembly);
        }

        public void AddAllTypesOf<PLUGINTYPE>()
        {
            AddAllTypesOf(typeof(PLUGINTYPE));
        }

        public void AddAllTypesOf(Type pluginType)
        {
            With(new FindAllTypesFilter(pluginType));
        }

    }
}