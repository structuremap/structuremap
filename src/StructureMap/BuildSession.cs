using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class BuildSession : IBuildSession
    {
        public static readonly string DEFAULT = "Default";

        private readonly IPipelineGraph _pipelineGraph;
        private readonly ISessionCache _sessionCache;

        public BuildSession(IPipelineGraph pipelineGraph, string requestedName = null, ExplicitArguments args = null)
        {
            _pipelineGraph = pipelineGraph;

            _sessionCache = new SessionCache(this, args);


            RequestedName = requestedName ?? DEFAULT;
        }

        protected IPipelineGraph pipelineGraph
        {
            get { return _pipelineGraph; }
        }

        public string RequestedName { get; set; }

        public void BuildUp(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            var pluggedType = target.GetType();

            var plan = _pipelineGraph.Policies.ToBuildUpPlan(pluggedType, () => {
                return _pipelineGraph.Instances.GetDefault(pluggedType) as IConfiguredInstance
                           ?? new ConfiguredInstance(pluggedType);
            });

            plan.BuildUp(this, target);
        }

        public T GetInstance<T>()
        {
            return (T) GetInstance(typeof (T));
        }

        public object GetInstance(Type pluginType)
        {
            return _sessionCache.GetDefault(pluginType, _pipelineGraph);
        }

        public T GetInstance<T>(string name)
        {
            return (T) CreateInstance(typeof (T), name);
        }

        public object GetInstance(Type pluginType, string name)
        {
            return CreateInstance(pluginType, name);
        }

        public T TryGetInstance<T>() where T : class
        {
            return (T) TryGetInstance(typeof (T));
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return (T) TryGetInstance(typeof (T), name);
        }

        public object TryGetInstance(Type pluginType)
        {
            return _sessionCache.TryGetDefault(pluginType, _pipelineGraph);
        }

        public object TryGetInstance(Type pluginType, string name)
        {
            return _pipelineGraph.Instances.HasInstance(pluginType, name) ? ((IContext) this).GetInstance(pluginType, name) : null;
        }

        public IEnumerable<T> All<T>() where T : class
        {
            return _sessionCache.All<T>();
        }

        public object ResolveFromLifecycle(Type pluginType, Instance instance)
        {
            var cache = _pipelineGraph.DetermineLifecycle(pluginType, instance).FindCache(_pipelineGraph);
            return cache.Get(pluginType, instance, this);
        }

        public object BuildNewInSession(Type pluginType, Instance instance)
        {
            var plan = instance.ResolveBuildPlan(pluginType, _pipelineGraph.Policies);
            return plan.Build(this);
        }

        public object BuildNewInOriginalContext(Type pluginType, Instance instance)
        {
            var session = new BuildSession(pipelineGraph.Root(), requestedName: instance.Name);
            return session.BuildNewInSession(pluginType, instance);
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return _pipelineGraph.Instances.GetAllInstances(typeof (T)).Select(x => (T) FindObject(typeof (T), x)).ToArray();
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            var allInstances = _pipelineGraph.Instances.GetAllInstances(pluginType);
            return allInstances.Select(x => FindObject(pluginType, x)).ToArray();
        }

        public static BuildSession ForPluginGraph(PluginGraph graph, ExplicitArguments args = null)
        {
            var pipeline = PipelineGraph.BuildRoot(graph);
            
            return new BuildSession(pipeline, args: args);
        }

        public static BuildSession Empty(ExplicitArguments args = null)
        {
            return ForPluginGraph(new PluginGraph(), args);
        }

        public virtual object CreateInstance(Type pluginType, string name)
        {
            var instance = _pipelineGraph.Instances.FindInstance(pluginType, name);
            if (instance == null)
            {
                // TODO -- make sure there is a UT on this behavior
                throw new StructureMapConfigurationException("Could not find an Instance named '{0}' for PluginType {1}", name, pluginType.GetFullName());
            }

            return FindObject(pluginType, instance);
        }

        // This is where all Creation happens
        public virtual object FindObject(Type pluginType, Instance instance)
        {
            var lifecycle = _pipelineGraph.DetermineLifecycle(pluginType, instance);
            return _sessionCache.GetObject(pluginType, instance, lifecycle);
        }

        public Policies Policies
        {
            get { return _pipelineGraph.Policies; }
        }
    }
}