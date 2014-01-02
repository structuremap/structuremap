using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<IRegistrationConvention> _conventions = new List<IRegistrationConvention>();
        private readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();

        private readonly List<Action<PluginGraph>> _postScanningActions = new List<Action<PluginGraph>>();

        public int Count
        {
            get { return _assemblies.Count; }
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

        public void Convention<T>() where T : IRegistrationConvention, new()
        {
            var previous = _conventions.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
            {
                With(new T());
            }
        }

        public void LookForRegistries()
        {
            Convention<FindRegistriesScanner>();
        }

        public void TheCallingAssembly()
        {
            var callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        public void AssemblyContainingType<T>()
        {
            _assemblies.Add(typeof (T).Assembly);
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.Assembly);
        }

        public FindAllTypesFilter AddAllTypesOf<TPluginType>()
        {
            return AddAllTypesOf(typeof (TPluginType));
        }

        public FindAllTypesFilter AddAllTypesOf(Type pluginType)
        {
            var filter = new FindAllTypesFilter(pluginType);
            With(filter);

            return filter;
        }


        public void Exclude(Func<Type, bool> exclude)
        {
            _filter.Excludes += exclude;
        }

        public void ExcludeNamespace(string nameSpace)
        {
            Exclude(type => type.IsInNamespace(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            ExcludeNamespace(typeof (T).Namespace);
        }

        public void Include(Func<Type, bool> predicate)
        {
            _filter.Includes += predicate;
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

        public void ModifyGraphAfterScan(Action<PluginGraph> modifyGraph)
        {
            _postScanningActions.Add(modifyGraph);
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
            var assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                    Path.GetExtension(file).Equals(
                        ".exe",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    Path.GetExtension(file).Equals(
                        ".dll",
                        StringComparison.OrdinalIgnoreCase));

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

        public void With(IRegistrationConvention convention)
        {
            _conventions.Fill(convention);
        }

        public void ScanForTypes(TypePool types, PluginGraph pluginGraph)
        {
            var registry = new Registry();

            types.For(_assemblies, _filter).Each(
                type => _conventions.Each(c => c.Process(type, registry)));

            registry.As<IPluginGraphConfiguration>().Configure(pluginGraph);
            _postScanningActions.Each(x => x(pluginGraph));
        }


        public bool Contains(string assemblyName)
        {
            foreach (var assembly in _assemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return true;
                }
            }

            return false;
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }


        /// <summary>
        /// Adds the DefaultConventionScanner to the scanning operations.  I.e., a concrete
        /// class named "Something" that implements "ISomething" will be automatically 
        /// added to PluginType "ISomething"
        /// </summary>
        public ConfigureConventionExpression WithDefaultConventions()
        {
            var convention = new DefaultConventionScanner();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Scans for PluginType's and Concrete Types that close the given open generic type
        /// </summary>
        /// <example>
        /// 
        /// </example>
        /// <param name="openGenericType"></param>
        public ConfigureConventionExpression ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            var convention = new GenericConnectionScanner(openGenericType);
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Automatically registers all concrete types without primitive arguments
        /// against its first interface, if any
        /// </summary>
        public ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface()
        {
            var convention = new FirstInterfaceConvention();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Directs the scanning to automatically register any type that is the single
        /// implementation of an interface against that interface.
        /// The filters apply
        /// </summary>
        public ConfigureConventionExpression SingleImplementationsOfInterface()
        {
            var convention = new ImplementationMap();
            With(convention);
            ModifyGraphAfterScan(convention.RegisterSingleImplementations);
            return new ConfigureConventionExpression(convention);
        }

        public PluginGraph ToPluginGraph()
        {
            var builder = new PluginGraphBuilder();
            builder.AddScanner(this);
            return builder.Build();
        }
    }
}