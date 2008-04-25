using System;
using System.Diagnostics;
using System.Reflection;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class NormalGraphBuilder : IGraphBuilder
    {
        private readonly IInterceptorChainBuilder _builder;
        private MachineOverride _machine;
        private PluginGraph _pluginGraph;
        private Profile _profile;
        private PluginGraph _systemGraph;
        private InstanceManager _systemInstanceManager;


        public NormalGraphBuilder(Registry[] registries) : this(new InterceptorChainBuilder(), registries)
        {
        }

        public NormalGraphBuilder(IInterceptorChainBuilder builder, Registry[] registries)
        {
            _builder = builder;

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

        public PluginGraph CreatePluginGraph()
        {
            _pluginGraph.ReadDefaults();
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

        public void AddProfile(string profileName)
        {
            _profile = new Profile(profileName);
            _pluginGraph.DefaultManager.AddProfile(_profile);
        }

        public void OverrideProfile(string fullTypeName, string instanceKey)
        {
            _profile.AddOverride(fullTypeName, instanceKey);
        }

        public void AddMachine(string machineName, string profileName)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                _machine = new MachineOverride(machineName, null);
            }
            else
            {
                Profile profile = _pluginGraph.DefaultManager.GetProfile(profileName);

                if (profile == null)
                {
                    _pluginGraph.Log.RegisterError(195, profileName, machineName);
                    return;
                }

                _machine = new MachineOverride(machineName, profile);
            }


            _pluginGraph.DefaultManager.AddMachineOverride(_machine);
        }

        public void OverrideMachine(string fullTypeName, string instanceKey)
        {
            _machine.AddMachineOverride(fullTypeName, instanceKey);
        }

        public TypePath LocateOrCreateFamilyForType(string fullName)
        {
            return _pluginGraph.LocateOrCreateFamilyForType(fullName);
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

        public void AddPluginFamily(TypePath typePath, string defaultKey, InstanceScope scope)
        {
            Type pluginType;
            try
            {
                pluginType = typePath.FindType();
            }
            catch (Exception ex)
            {
                throw new StructureMapException(103, ex, typePath.ClassName, typePath.AssemblyName);
            }


            PluginFamily family = _pluginGraph.LocateOrCreateFamilyForType(pluginType);

            // Xml configuration wins
            family.DefaultInstanceKey = defaultKey;
            InterceptionChain interceptionChain = _builder.Build(scope);
            family.AddInterceptionChain(interceptionChain);
        }

        public virtual void AttachSource(TypePath pluginTypePath, InstanceMemento sourceMemento)
        {
            try
            {
                MementoSource source = (MementoSource) buildSystemObject(typeof (MementoSource), sourceMemento);
                AttachSource(pluginTypePath, source);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(120, ex, pluginTypePath);
            }
        }

        public void AttachSource(TypePath pluginTypePath, MementoSource source)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginTypePath.FindType()];
            family.AddMementoSource(source);
        }

        public Plugin AddPlugin(TypePath pluginTypePath, TypePath pluginPath, string concreteKey)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginTypePath.FindType()];
            if (family == null)
            {
                string message =
                    string.Format("Could not find a PluginFamily for {0}", pluginTypePath.AssemblyQualifiedName);
                throw new ApplicationException(message);
            }

            Plugin plugin = new Plugin(pluginPath, concreteKey);
            family.Plugins.Add(plugin);

            return plugin;
        }

        public SetterProperty AddSetter(TypePath pluginTypePath, string concreteKey, string setterName)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginTypePath.FindType()];
            Plugin plugin = family.Plugins[concreteKey];
            return plugin.Setters.Add(setterName);
        }

        public virtual void AddInterceptor(TypePath pluginTypePath, InstanceMemento interceptorMemento)
        {
            PluginFamily family = _pluginGraph.PluginFamilies[pluginTypePath.FindType()];
            try
            {
                InstanceFactoryInterceptor interceptor =
                    (InstanceFactoryInterceptor)
                    buildSystemObject(typeof (InstanceFactoryInterceptor), interceptorMemento);

                family.InterceptionChain.AddInterceptor(interceptor);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(121, ex, pluginTypePath);
            }
        }

        public InstanceDefaultManager DefaultManager
        {
            get { return _pluginGraph.DefaultManager; }
        }

        public void RegisterMemento(TypePath pluginTypePath, InstanceMemento memento)
        {
            PluginFamily family = _pluginGraph.LocateOrCreateFamilyForType(pluginTypePath.FindType());
            family.AddInstance(memento);
        }

        #endregion

        private object buildSystemObject(Type type, InstanceMemento memento)
        {
            Instance instance = memento.ReadInstance(_systemGraph, type);
            return _systemInstanceManager.CreateInstance(type, instance);
        }
    }
}