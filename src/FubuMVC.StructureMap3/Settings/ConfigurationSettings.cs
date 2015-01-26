using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles;
using FubuMVC.Core.Registration;

namespace FubuMVC.StructureMap3.Settings
{
    [ApplicationLevel]
    public class ConfigurationSettings
    {
        private readonly SettingRegistry _registry = new SettingRegistry();
        private readonly IList<InclusionExpression> _inclusions = new List<InclusionExpression>();

        internal SettingRegistry BuildRegistry(BehaviorGraph graph)
        {
            _inclusions.Each(x => x.Apply(_registry, graph));

            return _registry;
        }

        public void Include<T>() where T : class, new()
        {
            _registry.AddSettingType<T>();
        }

        public InclusionExpression IncludeAllClassesSuffixedBySetting()
        {
            return IncludeClasses(SettingsScanner.DefaultFilter);
        }

        public InclusionExpression IncludeClasses(Func<Type, bool> filter)
        {
            var inclusion = new InclusionExpression(filter);
            _inclusions.Add(inclusion);

            return inclusion;
        }

        public class InclusionExpression
        {
            private readonly Func<Type, bool> _filter;
            private readonly IList<Assembly> _assemblies = new List<Assembly>();
            private bool _useApplicationAssembly;

            public InclusionExpression(Func<Type, bool> filter)
            {
                _filter = filter;
            }

            internal void Apply(SettingRegistry registry, BehaviorGraph graph)
            {
                var typePool = new TypePool();
                if (_useApplicationAssembly)
                {
                    typePool.AddAssembly(graph.ApplicationAssembly);
                }

                typePool.AddAssemblies(_assemblies);

                typePool.TypesMatching(_filter)
                    .Where(type => graph.Services.DefaultServiceFor(type) == null)
                    .Each(type => registry.AddSettingType(type));
            }

            public InclusionExpression FromTheApplicationAssembly()
            {
                _useApplicationAssembly = true;

                return this;
            }

            /// <summary>
            /// USE WITH CAUTION!
            /// </summary>
            /// <returns></returns>
            public InclusionExpression FromAllPackageAssemblies()
            {
                _assemblies.AddRange(PackageRegistry.PackageAssemblies);
                return this;
            }

            public InclusionExpression FromAssembly(Assembly assembly)
            {
                _assemblies.Add(assembly);
                return this;
            }
        }
    }
}