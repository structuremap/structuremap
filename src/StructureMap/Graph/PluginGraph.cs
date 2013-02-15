using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Util;
using StructureMap;

namespace StructureMap.Graph
{
    public interface IPluginGraph
    {
        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        /// <param name = "name"></param>
        void AddType(Type pluginType, Type concreteType, string name);
    }

    public interface IFamilyPolicy
    {
        PluginFamily Build(Type type);
    }

    /// <summary>
    ///   Models the runtime configuration of a StructureMap Container
    /// </summary>
    [Serializable]
    public class PluginGraph : IPluginGraph
    {
        private readonly Cache<Type, PluginFamily> _families;
        private readonly IList<IFamilyPolicy> _policies = new List<IFamilyPolicy>();

        private readonly InterceptorLibrary _interceptorLibrary = new InterceptorLibrary();
        private readonly ProfileManager _profileManager = new ProfileManager();
        private readonly List<Registry> _registries = new List<Registry>();
        private GraphLog _log = new GraphLog();

        public PluginGraph()
        {
            _families = new Cache<Type, PluginFamily>(type =>
            {
                return _policies.FirstValue(x => x.Build(type)) ?? new PluginFamily(type);
            });
        }

        public static PluginGraph Empty()
        {
            return new PluginGraphBuilder().Build();
        }

        // TODO -- might simplify this later
        public void AddFamilyPolicy(IFamilyPolicy policy)
        {
            _policies.Add(policy);
        }

        public List<Registry> Registries
        {
            get { return _registries; }
        }

        public ProfileManager ProfileManager
        {
            get { return _profileManager; }
        }

        public GraphLog Log
        {
            get { return _log; }
            set { _log = value; }
        }

        // TODO -- do something tighter here later
        public Cache<Type, PluginFamily> Families
        {
            get { return _families; }
        }

        public InterceptorLibrary InterceptorLibrary
        {
            get { return _interceptorLibrary; }
        }

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        public virtual void AddType(Type pluginType, Type concreteType)
        {
            _families[pluginType].AddType(concreteType);
        }

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        /// <param name = "name"></param>
        public virtual void AddType(Type pluginType, Type concreteType, string name)
        {
            _families[pluginType].AddType(concreteType, name);
        }

        public void SetDefault(string profileName, Type pluginType, Instance instance)
        {
            _families[pluginType].AddInstance(instance);
            _profileManager.SetDefault(profileName, pluginType, instance);
        }

        /// <summary>
        ///   Add configuration to a PluginGraph with the Registry DSL
        /// </summary>
        /// <param name = "action"></param>
        public void Configure(Action<Registry> action)
        {
            var registry = new Registry();
            action(registry);

            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public void ImportRegistry(Type type)
        {
            if (Registries.Any(x => x.GetType() == type)) return;

            var registry = (Registry) Activator.CreateInstance(type);
            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public static PluginGraph BuildGraphFromAssembly(Assembly assembly)
        {
            var builder = new PluginGraphBuilder();
            var scanner = new AssemblyScanner();
            scanner.Assembly(assembly);

            builder.AddScanner(scanner);

            return builder.Build();
        }

        public void AddFamily(PluginFamily family)
        {
            _families[family.PluginType] = family;
        }
    }
}