using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class BuildSession : IBuildSession, IContext
    {
        public static readonly string DEFAULT = "Default";

        private readonly ISessionCache _sessionCache;

        private readonly Stack<Instance> _instances;

        public BuildSession(IPipelineGraph pipelineGraph, string requestedName = null, ExplicitArguments args = null)
            : this(pipelineGraph, requestedName, args, null)
        {
        }

        private BuildSession(IPipelineGraph pipelineGraph, string requestedName, ExplicitArguments args,
            Stack<Instance> buildStack)
        {
            this.pipelineGraph = pipelineGraph;

            _sessionCache = new SessionCache(this, args);

            RequestedName = requestedName ?? DEFAULT;

            _instances = buildStack ?? new Stack<Instance>();
        }

        protected IPipelineGraph pipelineGraph { get; }

        public string RequestedName { get; set; }

        public void BuildUp(object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var pluggedType = target.GetType();

            var plan = pipelineGraph.Policies.ToBuildUpPlan(pluggedType, () =>
            {
                return pipelineGraph.Instances.GetDefault(pluggedType) as IConfiguredInstance
                       ?? new ConfiguredInstance(pluggedType);
            });

            plan.BuildUp(this, this, target);
        }

        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public object GetInstance(Type pluginType)
        {
            return _sessionCache.GetDefault(pluginType, pipelineGraph);
        }

        public T GetInstance<T>(string name)
        {
            return (T)CreateInstance(typeof(T), name);
        }

        public object GetInstance(Type pluginType, string name)
        {
            return CreateInstance(pluginType, name);
        }

        public T TryGetInstance<T>() where T : class
        {
            return (T)TryGetInstance(typeof(T));
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return (T)TryGetInstance(typeof(T), name);
        }

        public object TryGetInstance(Type pluginType)
        {
            return _sessionCache.TryGetDefault(pluginType, pipelineGraph);
        }

        public object TryGetInstance(Type pluginType, string name)
        {
            return pipelineGraph.Instances.HasInstance(pluginType, name)
                ? ((IContext)this).GetInstance(pluginType, name)
                : null;
        }

        public IEnumerable<T> All<T>() where T : class
        {
            return _sessionCache.All<T>();
        }

        public object ResolveFromLifecycle(Type pluginType, Instance instance)
        {
            var cache = pipelineGraph.DetermineLifecycle(pluginType, instance).FindCache(pipelineGraph);
            return cache.Get(pluginType, instance, this);
        }

        public object BuildNewInSession(Type pluginType, Instance instance)
        {
            if (pluginType == null) throw new ArgumentNullException(nameof(pluginType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (RootType == null) RootType = instance.ReturnedType;

            var plan = instance.ResolveBuildPlan(pluginType, pipelineGraph.Policies);
            return plan.Build(this, this);
        }

        public object BuildUnique(Type pluginType, Instance instance)
        {
            var @object = BuildNewInSession(pluginType, instance);
            if (@object is IDisposable && pipelineGraph.Role == ContainerRole.Nested)
            {
                pipelineGraph.TrackDisposable((IDisposable)@object);
            }

            return @object;
        }

        public object BuildNewInOriginalContext(Type pluginType, Instance instance)
        {
            var session = new BuildSession(pipelineGraph.Root(), instance.Name, null, _instances);
            return session.BuildNewInSession(pluginType, instance);
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return
                pipelineGraph.Instances.GetAllInstances(typeof(T))
                    .Select(x => (T)FindObject(typeof(T), x))
                    .ToArray();
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            var allInstances = pipelineGraph.Instances.GetAllInstances(pluginType);
            return allInstances.Select(x => FindObject(pluginType, x)).ToArray();
        }

        public static BuildSession ForPluginGraph(PluginGraph graph, ExplicitArguments args = null)
        {
            var pipeline = PipelineGraph.BuildRoot(graph);

            return new BuildSession(pipeline, args: args);
        }

        public static BuildSession Empty(ExplicitArguments args = null)
        {
            return ForPluginGraph(PluginGraph.CreateRoot(), args);
        }

        public virtual object CreateInstance(Type pluginType, string name)
        {
            var instance = pipelineGraph.Instances.FindInstance(pluginType, name);
            if (instance == null)
            {
                var ex =
                    new StructureMapConfigurationException("Could not find an Instance named '{0}' for PluginType {1}",
                        name, pluginType.GetFullName());

                ex.Context = new WhatDoIHaveWriter(pipelineGraph).GetText(new ModelQuery { PluginType = pluginType },
                    "The current configuration for type {0} is:".ToFormat(pluginType.GetFullName()));

                throw ex;
            }

            RootType = instance.ReturnedType;

            return FindObject(pluginType, instance);
        }

        public void Push(Instance instance)
        {
            if (_instances.Contains(instance))
            {
                throw new StructureMapBuildException("Bi-directional dependency relationship detected!" +
                                                     Environment.NewLine + "Check the StructureMap stacktrace below:");
            }

            _instances.Push(instance);
        }

        public void Pop()
        {
            if (_instances.Any())
            {
                _instances.Pop();
            }
        }

        public Type ParentType
        {
            get
            {
                if (_instances.Count > 1)
                {
                    return _instances.ToArray().Skip(1).First().ReturnedType;
                }

                return null;
            }
        }

        public Type RootType { get; internal set; }

        // This is where all Creation happens
        public virtual object FindObject(Type pluginType, Instance instance)
        {
            RootType = instance.ReturnedType;
            var lifecycle = pipelineGraph.DetermineLifecycle(pluginType, instance);
            return _sessionCache.GetObject(pluginType, instance, lifecycle);
        }

        public object BuildWithExplicitArgs(Type pluginType, Instance instance)
        {
            RootType = instance.ReturnedType;
            return _sessionCache.GetObject(pluginType, instance, Lifecycles.Unique);
        }

        public Policies Policies => pipelineGraph.Policies;
    }
}