using System;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly PluginGraph _pluginGraph;
        private readonly PluginGraph _systemGraph;
        private Profile _profile;
        private InstanceManager _systemInstanceManager;


        public GraphBuilder(Registry[] registries) : this(registries, new PluginGraph())
        {
        }

        public GraphBuilder(Registry[] registries, PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            foreach (Registry registry in registries)
            {
                registry.ConfigurePluginGraph(_pluginGraph);
            }

            _systemGraph = new PluginGraph(false);
            _systemGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
        }

        #region IGraphBuilder Members

        public void FinishFamilies()
        {
            _pluginGraph.Seal();
        }

        public PluginGraph SystemGraph
        {
            get { return _systemGraph; }
        }

        public PluginGraph PluginGraph
        {
            get { return _pluginGraph; }
        }

        public void AddAssembly(string assemblyName)
        {
            try
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
                _pluginGraph.Assemblies.Add(assembly);
                _systemGraph.Assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                _pluginGraph.Log.RegisterError(101, ex, assemblyName);
            }
        }

        public void PrepareSystemObjects()
        {
            _systemGraph.Seal();
        }

        public IProfileBuilder GetProfileBuilder()
        {
            return new ProfileBuilder(_pluginGraph);
        }

        public void ConfigureFamily(TypePath pluginTypePath, Action<PluginFamily> action)
        {
            try
            {
                Type pluginType = pluginTypePath.FindType();
                PluginFamily family = _pluginGraph.FindFamily(pluginType);
                action(family);
            }
            catch (Exception ex)
            {
                _pluginGraph.Log.RegisterError(103, ex, pluginTypePath.AssemblyQualifiedName);
            }
        }


        public void WithSystemObject<T>(InstanceMemento memento, string context, Action<T> action)
        {
            try
            {
                T systemObject = (T) buildSystemObject(typeof (T), memento);
                action(systemObject);
            }
            catch (Exception ex)
            {
                _pluginGraph.Log.RegisterError(130, ex, context);
            }
        }

        public void WithType(TypePath path, string context, Action<Type> action)
        {
            _pluginGraph.Log.WithType(path, context, action);
        }

        #endregion

        private object buildSystemObject(Type type, InstanceMemento memento)
        {
            Instance instance = memento.ReadInstance(_systemGraph, type);

            if (_systemInstanceManager == null)
            {
                _systemInstanceManager = new InstanceManager(_systemGraph);
            }

            return _systemInstanceManager.GetInstance(type, instance);
        }
    }
}