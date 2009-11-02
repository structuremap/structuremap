using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public interface IAssemblyScanner
    {
        #region Designating Assemblies

        /// <summary>
        /// Add an Assembly to the scanning operation
        /// </summary>
        /// <param name="assembly"></param>
        void Assembly(Assembly assembly);

        /// <summary>
        /// Add an Assembly by name to the scanning operation
        /// </summary>
        /// <param name="assemblyName"></param>
        void Assembly(string assemblyName);

        /// <summary>
        /// Add the currently executing Assembly to the scanning operation
        /// </summary>
        void TheCallingAssembly();

        /// <summary>
        /// Add the Assembly that contains type T to the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AssemblyContainingType<T>();

        /// <summary>
        /// Add the Assembly that contains type to the scanning operation
        /// </summary>
        /// <param name="type"></param>
        void AssemblyContainingType(Type type);

        /// <summary>
        /// Sweep the designated path and add any Assembly's found in this folder to the
        /// scanning operation
        /// </summary>
        /// <param name="path"></param>
        void AssembliesFromPath(string path);
        
        /// <summary>
        /// Sweep the designated path and add any Assembly's found in this folder to the
        /// scanning operation.  The assemblyFilter can be used to filter or limit the 
        /// Assembly's that are picked up.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assemblyFilter"></param>
        void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter);

        /// <summary>
        /// Sweep the application base directory of current app domain and add any Assembly's 
        /// found to the scanning operation.
        /// </summary>
        void AssembliesFromApplicationBaseDirectory();

        /// <summary>
        /// Sweep the application base directory of current app domain and add any Assembly's 
        /// found to the scanning operation. The assemblyFilter can be used to filter or limit the 
        /// Assembly's that are picked up.
        /// </summary>
        void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter);

        #endregion

        // ... Other methods

        #region Adding TypeScanners

        /// <summary>
        /// Adds an ITypeScanner object to the scanning operation
        /// </summary>
        /// <param name="scanner"></param>
        void With(ITypeScanner scanner);

        void With(IHeavyweightTypeScanner heavyweightScanner);
        /// <summary>
        /// Adds the DefaultConventionScanner to the scanning operations.  I.e., a concrete
        /// class named "Something" that implements "ISomething" will be automatically 
        /// added to PluginType "ISomething"
        /// </summary>
        void WithDefaultConventions();

        /// <summary>
        /// Creates and adds a new ITypeScanner of type T to this scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void With<T>() where T : ITypeScanner, new();

        #endregion

        #region Other options

        /// <summary>
        /// Directs the scanning operation to automatically detect and include any Registry
        /// classes found in the Assembly's being scanned
        /// </summary>
        void LookForRegistries();

        /// <summary>
        /// Add all concrete types of the Plugin Type as Instances of Plugin Type
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        FindAllTypesFilter AddAllTypesOf<PLUGINTYPE>();

        /// <summary>
        /// Add all concrete types of the Plugin Type as Instances of Plugin Type
        /// </summary>
        /// <param name="pluginType"></param>
        FindAllTypesFilter AddAllTypesOf(Type pluginType);

        /// <summary>
        /// Makes this scanning operation ignore all [PluginFamily] and [Pluggable] attributes
        /// </summary>
        void IgnoreStructureMapAttributes();

        #endregion

        #region Filtering types

        /// <summary>
        /// Exclude types that match the Predicate from being scanned
        /// </summary>
        /// <param name="exclude"></param>
        void Exclude(Predicate<Type> exclude);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void ExcludeNamespace(string nameSpace);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeNamespaceContainingType<T>();

        /// <summary>
        /// Only include types matching the Predicate in the scanning operation. You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="predicate"></param>
        void Include(Predicate<Type> predicate);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void IncludeNamespace(string nameSpace);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IncludeNamespaceContainingType<T>();

        /// <summary>
        /// Exclude this specific type from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeType<T>();

        // ... Other methods
        #endregion

        /// <summary>
        /// Scans for PluginType's and Concrete Types that close the given open generic type
        /// </summary>
        /// <example>
        /// 
        /// </example>
        /// <param name="openGenericType"></param>
        void ConnectImplementationsToTypesClosing(Type openGenericType);
    }

    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<Predicate<Type>> _excludes = new List<Predicate<Type>>();
        private readonly List<Predicate<Type>> _includes = new List<Predicate<Type>>();
        private readonly List<ITypeScanner> _scanners = new List<ITypeScanner>();
        private readonly List<IHeavyweightTypeScanner> _heavyweightScanners = new List<IHeavyweightTypeScanner>();

        public AssemblyScanner()
        {
            With<FamilyAttributeScanner>();
            With<PluggableAttributeScanner>();
        }

        public int Count
        {
            get { return _assemblies.Count; }
        }


        internal void ScanForAll(PluginGraph pluginGraph)
        {
            var heavyweightScan = configureHeavyweightScan();

            _assemblies.ForEach(assem => scanTypesInAssembly(assem, pluginGraph));

            performHeavyweightScan(pluginGraph, heavyweightScan);
        }

        private void scanTypesInAssembly(Assembly assembly, PluginGraph graph)
        {
            try
            {
                foreach (var type in assembly.GetExportedTypes())
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

        private TypeMapBuilder configureHeavyweightScan()
        {
            var typeMapBuilder = new TypeMapBuilder();
            if (_heavyweightScanners.Count > 0)
            {
                With(typeMapBuilder);
            }
            return typeMapBuilder;
        }

        private void performHeavyweightScan(PluginGraph pluginGraph, TypeMapBuilder typeMapBuilder)
        {
            var typeMaps = typeMapBuilder.GetTypeMaps();
            _heavyweightScanners.ForEach(scanner => scanner.Process(pluginGraph, typeMaps));
            typeMapBuilder.Dispose();
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
        
        public void With(IHeavyweightTypeScanner heavyweightScanner)
        {
            if (_heavyweightScanners.Contains(heavyweightScanner)) return;

            _heavyweightScanners.Add(heavyweightScanner);
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
            var trace = new StackTrace(false);

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
            _assemblies.Add(typeof (T).Assembly);
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.Assembly);
        }

        public FindAllTypesFilter AddAllTypesOf<PLUGINTYPE>()
        {
            return AddAllTypesOf(typeof (PLUGINTYPE));
        }

        public FindAllTypesFilter AddAllTypesOf(Type pluginType)
        {
            var filter = new FindAllTypesFilter(pluginType);
            With(filter);

            return filter;
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
            ExcludeNamespace(typeof (T).Namespace);
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

        public void ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            With(new GenericConnectionScanner(openGenericType));
        }

        public void AssembliesFromApplicationBaseDirectory()
        {
            AssembliesFromApplicationBaseDirectory(a => true);
        }

        public void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            AssembliesFromPath(baseDirectory, assemblyFilter);
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (Directory.Exists(binPath))
            {
                AssembliesFromPath(binPath, assemblyFilter);
            }

            
        }

        public void AssembliesFromPath(string path)
        {
            AssembliesFromPath(path, a => true);
        }

        public void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter)
        {
            var assemblyPaths = System.IO.Directory.GetFiles(path).Where(file =>
                                                                         System.IO.Path.GetExtension(file).Equals(
                                                                             ".exe", StringComparison.OrdinalIgnoreCase)
                                                                         ||
                                                                         System.IO.Path.GetExtension(file).Equals(
                                                                             ".dll", StringComparison.OrdinalIgnoreCase));

            foreach (var assemblyPath in assemblyPaths)
            {
                Assembly assembly = null;
                try
                {
                    assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                }
                if (assembly != null && assemblyFilter(assembly)) Assembly(assembly);
            }
        }
    }
}
