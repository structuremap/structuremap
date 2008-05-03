using System;
using System.Reflection;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    // TODO:  Kill in 3.5
    public delegate void Action<T>(T subject);

    public class NormalGraphBuilder : IGraphBuilder
    {
        private readonly PluginGraph _pluginGraph;
        private readonly PluginGraph _systemGraph;
        private Profile _profile;
        private InstanceManager _systemInstanceManager;


        public NormalGraphBuilder(Registry[] registries)
        {
            _pluginGraph = new PluginGraph();
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

        [Obsolete("Do away?")] public PluginGraph CreatePluginGraph()
        {
            return _pluginGraph;
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
            _systemGraph.Seal();
            _systemInstanceManager = new InstanceManager(_systemGraph);
        }

        // TODO:  Cleanup
        //public void AddPluginFamily(Type pluginType, string defaultKey, InstanceScope scope)
        //{
        //    PluginFamily family = _pluginGraph.FindFamily(pluginType);

        //    // Xml configuration wins
        //    family.DefaultInstanceKey = defaultKey;
        //    family.SetScopeTo(scope);
        //}

        public virtual void AttachSource(Type pluginType, InstanceMemento sourceMemento)
        {
            try
            {
                MementoSource source = (MementoSource) buildSystemObject(typeof (MementoSource), sourceMemento);
                AttachSource(pluginType, source);
            }
            catch (Exception ex)
            {
                // TODO:  put error in PluginGraph
                throw new StructureMapException(120, ex, TypePath.GetAssemblyQualifiedName(pluginType));
            }
        }

        public void AttachSource(Type pluginType, MementoSource source)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginType];
            family.AddMementoSource(source);
        }

        public Plugin AddPlugin(Type pluginType, TypePath pluginPath, string concreteKey)
        {
            // TODO:  Make this go through PluginGraph.FindFamily()
            PluginFamily family = _pluginGraph.PluginFamilies[pluginType];
            if (family == null)
            {
                string message =
                    string.Format("Could not find a PluginFamily for {0}", pluginType.AssemblyQualifiedName);

                // TODO:  put error in PluginGraph
                throw new ApplicationException(message);
            }

            Plugin plugin = new Plugin(pluginPath, concreteKey);
            family.Plugins.Add(plugin);

            return plugin;
        }

        public SetterProperty AddSetter(Type pluginType, string concreteKey, string setterName)
        {
            // TODO:  Make this go through PluginGraph.FindFamily()
            PluginFamily family = _pluginGraph.PluginFamilies[pluginType];
            Plugin plugin = family.Plugins[concreteKey];
            return plugin.Setters.Add(setterName);
        }

        public virtual void AddInterceptor(Type pluginType, InstanceMemento interceptorMemento)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginType];
            try
            {
                IInstanceInterceptor interceptor =
                    (IInstanceInterceptor)
                    buildSystemObject(typeof (IInstanceInterceptor), interceptorMemento);

                family.AddInterceptor(interceptor);
            }
            catch (Exception ex)
            {
                // TODO:  put error in PluginGraph
                throw new StructureMapException(121, ex, TypePath.GetAssemblyQualifiedName(pluginType));
            }
        }

        public void RegisterMemento(Type pluginType, InstanceMemento memento)
        {
            PluginFamily family = _pluginGraph.FindFamily(pluginType);
            family.AddInstance(memento);
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

        #endregion

        private object buildSystemObject(Type type, InstanceMemento memento)
        {
            Instance instance = memento.ReadInstance(_systemGraph, type);
            return _systemInstanceManager.CreateInstance(type, instance);
        }
    }
}