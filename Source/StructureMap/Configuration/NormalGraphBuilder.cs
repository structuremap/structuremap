using System;
using System.Reflection;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;
using StructureMap.Interceptors;

namespace StructureMap.Configuration
{
	public class NormalGraphBuilder : IGraphBuilder
	{
		private PluginGraph _pluginGraph;
		private PluginGraph _systemGraph;
		private InstanceManager _systemInstanceManager;
		private InstanceDefaultManager _defaultManager;
		private readonly IInterceptorChainBuilder _builder;


		public NormalGraphBuilder(InstanceDefaultManager defaultManager) : this(defaultManager, new InterceptorChainBuilder()){}

		public NormalGraphBuilder(InstanceDefaultManager defaultManager, IInterceptorChainBuilder builder)
		{
			_defaultManager = defaultManager;
			_builder = builder;
			_pluginGraph = new PluginGraph();
			_systemGraph = new PluginGraph();
			_systemGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
		}


		public void FinishFamilies()
		{
			_pluginGraph.Seal();
		}

		public PluginGraph CreatePluginGraph()
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


		public void AddAssembly(string assemblyName, string[] deployableTargets)
		{
			AssemblyGraph assemblyGraph = new AssemblyGraph(assemblyName);
			assemblyGraph.DeploymentTargets = deployableTargets;
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

		public void AddPluginFamily(TypePath typePath, string defaultKey, string[] deploymentTargets, InstanceScope scope)
		{	
			PluginFamily family = new PluginFamily(typePath, defaultKey);
			family.DefinitionSource = DefinitionSource.Explicit;
			family.InterceptionChain = _builder.Build(scope);
			_pluginGraph.PluginFamilies.Add(family);
		}

		private object buildSystemObject(Type type, InstanceMemento memento)
		{
			return _systemInstanceManager.CreateInstance(type, memento);
		}

		public virtual void AttachSource(string pluginTypeName, InstanceMemento sourceMemento)
		{
			try
			{
				MementoSource source = (MementoSource) buildSystemObject(typeof(MementoSource), sourceMemento);
				AttachSource(pluginTypeName, source);
			}
			catch (Exception ex)
			{
				throw new StructureMapException(120, ex, pluginTypeName);
			}
		}

		public void AttachSource(string pluginTypeName, MementoSource source)
		{
			PluginFamily family = FindPluginFamily(pluginTypeName);
			family.Source = source;
		}

		public PluginFamily FindPluginFamily(string pluginTypeName)
		{
			return _pluginGraph.PluginFamilies[pluginTypeName];
		}

		public Plugin AddPlugin(string pluginTypeName, TypePath pluginPath, string concreteKey)
		{
			PluginFamily family = _pluginGraph.PluginFamilies[pluginTypeName];
			Plugin plugin = new Plugin(pluginPath, concreteKey);
			plugin.DefinitionSource = DefinitionSource.Explicit;
			family.Plugins.Add(plugin);

			return plugin;
		}

		public SetterProperty AddSetter(string pluginTypeName, string concreteKey, string setterName)
		{
			PluginFamily family = _pluginGraph.PluginFamilies[pluginTypeName];
			Plugin plugin = family.Plugins[concreteKey];
			return plugin.Setters.Add(setterName);
		}

		public virtual void AddInterceptor(string pluginTypeName, InstanceMemento interceptorMemento)
		{
			PluginFamily family = _pluginGraph.PluginFamilies[pluginTypeName];
			try
			{
				InstanceFactoryInterceptor interceptor = 
					(InstanceFactoryInterceptor) buildSystemObject(typeof(InstanceFactoryInterceptor), interceptorMemento);

				family.InterceptionChain.AddInterceptor(interceptor);
			}
			catch (Exception ex)
			{
				throw new StructureMapException(121, ex, pluginTypeName);
			}
		}

		public InstanceDefaultManager DefaultManager
		{
			get { return _defaultManager; }
		}

		public void RegisterMemento(string pluginTypeName, InstanceMemento memento)
		{
			PluginFamily family = _pluginGraph.PluginFamilies[pluginTypeName];

			family.Source.AddExternalMemento(memento);
		}

	}
}
