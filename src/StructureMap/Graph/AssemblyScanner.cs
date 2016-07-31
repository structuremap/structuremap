using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

#pragma warning disable 1591

namespace StructureMap.Graph
{
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();
        private readonly List<AssemblyScanRecord> _records = new List<AssemblyScanRecord>();

        public int Count => _assemblies.Count;

        public string Description { get; set; }


        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
                _assemblies.Add(assembly);
        }

        public void Assembly(string assemblyName)
        {
            Assembly(AssemblyLoader.ByName(assemblyName));
        }

        public List<IRegistrationConvention> Conventions { get; } = new List<IRegistrationConvention>();

        public void Convention<T>() where T : IRegistrationConvention, new()
        {
            var previous = Conventions.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
                With(new T());
        }

        public void LookForRegistries()
        {
            Convention<FindRegistriesScanner>();
        }

        public void AssemblyContainingType<T>()
        {
            AssemblyContainingType(typeof(T));
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.GetTypeInfo().Assembly);
        }

        public FindAllTypesFilter AddAllTypesOf<TPluginType>()
        {
            return AddAllTypesOf(typeof(TPluginType));
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
            ExcludeNamespace(typeof(T).Namespace);
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
            IncludeNamespace(typeof(T).Namespace);
        }

        public void ExcludeType<T>()
        {
            Exclude(type => type == typeof(T));
        }

        public void With(IRegistrationConvention convention)
        {
            Conventions.Fill(convention);
        }

        public ConfigureConventionExpression WithDefaultConventions()
        {
            var convention = new DefaultConventionScanner();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        public ConfigureConventionExpression ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            var convention = new GenericConnectionScanner(openGenericType);
            With(convention);

            return new ConfigureConventionExpression(convention);
        }

        public ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface()
        {
            var convention = new FirstInterfaceConvention();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        public ConfigureConventionExpression SingleImplementationsOfInterface()
        {
            var convention = new ImplementationMap();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        public void Describe(StringWriter writer)
        {
            writer.WriteLine(Description);
            writer.WriteLine("Assemblies");
            writer.WriteLine("----------");

            _records.OrderBy(x => x.Name).Each(x => writer.WriteLine("* " + x));
            writer.WriteLine();

            writer.WriteLine("Conventions");
            writer.WriteLine("--------");
            Conventions.Each(x => writer.WriteLine("* " + x));
        }

        public Task<Registry> ScanForTypes()
        {
            if (!Conventions.Any())
            {
                throw new StructureMapConfigurationException($"There are no {nameof(IRegistrationConvention)}'s in this scanning operation. ");
            }

            return TypeRepository.FindTypes(_assemblies, type => _filter.Matches(type)).ContinueWith(t =>
            {
                var registry = new Registry();

                _records.AddRange(t.Result.Records);

                Conventions.Each(x => x.ScanTypes(t.Result, registry));

                return registry;
            });
        }


        public bool Contains(string assemblyName)
        {
            return _assemblies
                .Select(assembly => new AssemblyName(assembly.FullName))
                .Any(aName => aName.Name == assemblyName);
        }

        public bool HasAssemblies()
        {
            return _assemblies.Any();
        }


        public void TheCallingAssembly()
        {
            var callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                Assembly(callingAssembly);
            }
            else
            {
                throw new StructureMapConfigurationException("Could not determine the calling assembly, you may need to explicitly call IAssemblyScanner.Assembly()");
            }
        }

        public void AssembliesFromApplicationBaseDirectory()
        {
            AssembliesFromApplicationBaseDirectory(a => true);
        }

        public void AssembliesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        /// <summary>
        /// Choosing option will direct StructureMap to *also* scan files ending in '*.exe'
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyFilter"></param>
        /// <param name="includeExeFiles"></param>
        public void AssembliesAndExecutablesFromApplicationBaseDirectory(Func<Assembly, bool> assemblyFilter = null)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesAndExecutablesFromPath(string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesFromPath(string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesAndExecutablesFromPath(string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true).Where(assemblyFilter);


            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

        public void AssembliesFromPath(string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false).Where(assemblyFilter);


            foreach (var assembly in assemblies)
            {
                Assembly(assembly);
            }
        }

#if NET45
        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var smAssembly = typeof(Registry).Assembly;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly && assembly != smAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }
#else
        private static Assembly findTheCallingAssembly()
        {
            string trace = Environment.StackTrace;

            var parts = trace.Split('\n');
            var candidate = parts[4].Trim().Substring(3);

            Assembly assembly = null;
            var names = candidate.Split('.');
            for (var i = names.Length - 2; i > 0; i--) {
                var possibility = string.Join(".", names.Take(i).ToArray());

                try 
                {

                    assembly = System.Reflection.Assembly.Load(new AssemblyName(possibility));
                    break;
                }
                catch (Exception e)
                {
                  // Nothing
                }
            }

            return assembly;
        }

#endif
    }
}

#pragma warning restore 1591