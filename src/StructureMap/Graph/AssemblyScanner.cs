using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Diagnostics;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

#pragma warning disable 1591

namespace StructureMap.Graph
{
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<IRegistrationConvention> _conventions = new List<IRegistrationConvention>();
        private readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();
        private readonly List<AssemblyScanRecord> _records = new List<AssemblyScanRecord>(); 

        public int Count
        {
            get { return _assemblies.Count; }
        }

        public string Description { get; set; }


        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }

        public void Assembly(string assemblyName)
        {
            Assembly(AssemblyLoader.ByName(assemblyName));
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
            _conventions.Each(x => writer.WriteLine("* " + x));
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
            return AddAllTypesOf(typeof (TPluginType));
        }

        public FindAllTypesFilter AddAllTypesOf(Type pluginType)
        {
            var filter = new FindAllTypesFilter(pluginType);
            With(filter);

            return filter;
        }

        public FindAllTypeImplementationsFilter AddAllTypeImplementationsOf(Type pluginType)
        {
            var filter = new FindAllTypeImplementationsFilter(pluginType);
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

        public void With(IRegistrationConvention convention)
        {
            _conventions.Fill(convention);
        }

        public Task<Registry> ScanForTypes()
        {
            return TypeRepository.FindTypes(_assemblies, type => _filter.Matches(type)).ContinueWith(t =>
            {
                var registry = new Registry();

                _records.AddRange(t.Result.Records);

                _conventions.Each(x => x.ScanTypes(t.Result, registry));

                return registry;
            });
        }


        public bool Contains(string assemblyName)
        {
            return _assemblies
                .Select(assembly => new AssemblyName(assembly.FullName))
                .Any(aName => aName.Name == assemblyName);
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

        public bool HasAssemblies()
        {
            return _assemblies.Any();
        }
    }


}


#pragma warning restore 1591