using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using StructureMap.Diagnostics;

namespace StructureMap.Graph
{
    public static class TypeExtensions
    {
        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            return type.Namespace.StartsWith(nameSpace);
        }
    }

    public class AssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<ITypeScanner> _scanners = new List<ITypeScanner>();
        private readonly List<Predicate<Type>> _includes = new List<Predicate<Type>>();
        private readonly List<Predicate<Type>> _excludes = new List<Predicate<Type>>();

        public AssemblyScanner()
        {
            With<FamilyAttributeScanner>();
            With<PluggableAttributeScanner>();
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
                    if (!isInTheIncludes(type)) continue;
                    if (isInTheExcludes(type)) continue;

                    _scanners.ForEach(scanner => scanner.Process(type, graph));
                }
            }
            catch (Exception ex)
            {
                graph.Log.RegisterError(170, ex, assembly.FullName);
            }
        }

        private bool isInTheExcludes(Type type)
        {
            if (_excludes.Count == 0) return false;

            foreach (var exclude in _excludes)
            {
                if (exclude(type)) return true;
            }

            return false;
        }

        private bool isInTheIncludes(Type type)
        {
            if (_includes.Count == 0) return true;


            foreach (var include in _includes)
            {
                if (include(type)) return true;
            }

            return false;
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

        public void WithDefaultConventions()
        {
            With<DefaultConventionScanner>();
        }

        public void With<T>() where T : ITypeScanner, new()
        {
            _scanners.RemoveAll(scanner => scanner is T);

            ITypeScanner previous = _scanners.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
            {
                With(new T());
            }
        }

        public void LookForRegistries()
        {
            With<FindRegistriesScanner>();
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

        public void IgnoreStructureMapAttributes()
        {
            _scanners.RemoveAll(scanner => scanner is FamilyAttributeScanner);
            _scanners.RemoveAll(scanner => scanner is PluggableAttributeScanner);
        }


        public void Exclude(Predicate<Type> exclude)
        {
            _excludes.Add(exclude);
        }

        public void ExcludeNamespace(string nameSpace)
        {
            Exclude(type => type.IsInNamespace(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            ExcludeNamespace(typeof(T).Namespace);
        }

        public void Include(Predicate<Type> predicate)
        {
            _includes.Add(predicate);
        }

        public void IncludeNamespace(string nameSpace)
        {
            Include(type => type.IsInNamespace(nameSpace));
        }

        public void IncludeNamespaceContainingType<T>()
        {
            IncludeNamespace(typeof (T).Namespace);
        }

        public void ExcludeType<T>()
        {
            Exclude(type => type == typeof (T));
        }
    }
}