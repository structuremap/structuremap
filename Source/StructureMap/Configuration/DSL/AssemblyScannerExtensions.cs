using System;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    /// <summary>
    /// Extend the assembly scanning DSL to support the built-in registration conventions
    /// </summary>
    public static class AssemblyScannerExtensions
    {
        /// <summary>
        /// Adds the DefaultConventionScanner to the scanning operations.  I.e., a concrete
        /// class named "Something" that implements "ISomething" will be automatically 
        /// added to PluginType "ISomething"
        /// </summary>
        public static ConfigureConventionExpression WithDefaultConventions(this IAssemblyScanner assemblyScanner)
        {
            var convention = new DefaultConventionScanner();
            assemblyScanner.With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Scans for PluginType's and Concrete Types that close the given open generic type
        /// </summary>
        /// <example>
        /// 
        /// </example>
        /// <param name="openGenericType"></param>
        public static ConfigureConventionExpression ConnectImplementationsToTypesClosing(this IAssemblyScanner assemblyScanner, Type openGenericType)
        {
            var convention = new GenericConnectionScanner(openGenericType);
            assemblyScanner.With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Automatically registers all concrete types without primitive arguments
        /// against its first interface, if any
        /// </summary>
        public static ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface(this IAssemblyScanner assemblyScanner)
        {
            var convention = new FirstInterfaceConvention();
            assemblyScanner.With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Directs the scanning to automatically register any type that is the single
        /// implementation of an interface against that interface.
        /// The filters apply
        /// </summary>
        public static ConfigureConventionExpression SingleImplementationsOfInterface(this IAssemblyScanner assemblyScanner)
        {
            var convention = new ImplementationMap();
            assemblyScanner.With(convention);
            assemblyScanner.ModifyGraphAfterScan(convention.RegisterSingleImplementations);
            return new ConfigureConventionExpression(convention);
        }

    }
}