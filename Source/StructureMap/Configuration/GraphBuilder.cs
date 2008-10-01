using System;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly AssemblyScanner _assemblyScanner;
        private readonly PluginGraph _pluginGraph;
        private readonly PluginGraph _systemGraph;
        private readonly AssemblyScanner _systemScanner;
        private Profile _profile;
        private Container _systemContainer;


        public GraphBuilder(Registry[] registries) : this(registries, new PluginGraph())
        {
        }

        public GraphBuilder(Registry[] registries, PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            _assemblyScanner = new AssemblyScanner();
            _pluginGraph.AddScanner(_assemblyScanner);

            foreach (Registry registry in registries)
            {
                registry.ConfigurePluginGraph(_pluginGraph);
            }

            _systemScanner = new AssemblyScanner();
            _systemScanner.Assembly(Assembly.GetExecutingAssembly());
            _systemScanner.IgnoreRegistries();

            _systemGraph = new PluginGraph(_systemScanner);
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
                _assemblyScanner.Assembly(assembly);
                _systemScanner.Assembly(assembly);
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
                var systemObject = (T) buildSystemObject(typeof (T), memento);
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

            if (_systemContainer == null)
            {
                _systemContainer = new Container(_systemGraph);
            }

            return _systemContainer.GetInstance(type, instance);
        }
    }
}