using System;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class NormalGraphBuilder : IGraphBuilder
    {
        private readonly PluginGraph _pluginGraph;
        private readonly PluginGraph _systemGraph;
        private Profile _profile;
        private InstanceManager _systemInstanceManager;


        public NormalGraphBuilder(Registry[] registries) : this(registries, new PluginGraph())
        {
        }

        public NormalGraphBuilder(Registry[] registries, PluginGraph pluginGraph)
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
            AssemblyGraph assemblyGraph = new AssemblyGraph(assemblyName);
            _pluginGraph.Assemblies.Add(assemblyGraph);

            AssemblyGraph systemAssemblyGraph = new AssemblyGraph(assemblyName);
            systemAssemblyGraph.LookForPluginFamilies = false;
            _systemGraph.Assemblies.Add(systemAssemblyGraph);
        }

        public void StartFamilies()
        {
            // TODO:  is this a problem here?
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
                _pluginGraph.Log.RegisterError(103, ex, pluginTypePath.ClassName, pluginTypePath.AssemblyName);
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
            try
            {
                Type type = path.FindType();
                action(type);
            }
            catch (Exception ex)
            {
                _pluginGraph.Log.RegisterError(131, ex, path.AssemblyQualifiedName, context);
            }
        }

        #endregion

        private object buildSystemObject(Type type, InstanceMemento memento)
        {
            Instance instance = memento.ReadInstance(_systemGraph, type);

            if (_systemInstanceManager == null)
            {
                _systemInstanceManager = new InstanceManager(_systemGraph);
            }

            return _systemInstanceManager.CreateInstance(type, instance);
        }
    }
}