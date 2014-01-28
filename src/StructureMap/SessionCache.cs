using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    public interface ISessionCache
    {
        object GetDefault(Type pluginType, IPipelineGraph pipelineGraph);
        object GetObject(Type pluginType, Instance instance, ILifecycle lifecycle);
        object TryGetDefault(Type pluginType, IPipelineGraph pipelineGraph);

        IEnumerable<T> All<T>();
    }

    public class SessionCache : ISessionCache
    {
        private readonly IDictionary<int, object> _cachedObjects = new Dictionary<int, object>();
        private readonly IDictionary<Type, object> _defaults = new Dictionary<Type, object>();
        private readonly IBuildSession _resolver;

        public SessionCache(IBuildSession resolver)
        {
            _resolver = resolver;
        }

        public SessionCache(IBuildSession resolver, ExplicitArguments arguments)
            : this(resolver)
        {
            if (arguments != null) _defaults = arguments.Defaults;
        }

        public object GetDefault(Type pluginType, IPipelineGraph pipelineGraph)
        {
            if (_defaults.ContainsKey(pluginType))
            {
                return _defaults[pluginType];
            }

            var instance = pipelineGraph.Instances.GetDefault(pluginType);
            if (instance == null)
            {
                var ex =
                    new StructureMapConfigurationException(
                        "No default Instance is registered and cannot be automatically determined for type '{0}'",
                        pluginType.GetFullName());

                if (pipelineGraph.ToModel().HasImplementationsFor(pluginType))
                {
                    ex.Context = new WhatDoIHaveWriter(pipelineGraph).GetText(new ModelQuery {PluginType = pluginType},
                        "No default instance is specified.  The current configuration for type {0} is:".ToFormat(
                            pluginType.GetFullName()));
                }
                else
                {
                    ex.Context = "There is no configuration specified for " + pluginType.GetFullName();
                }


                throw ex;
            }

            var lifecycle = pipelineGraph.DetermineLifecycle(pluginType, instance);

            var o = GetObject(pluginType, instance, lifecycle);

            if (!(lifecycle is UniquePerRequestLifecycle))
            {
                _defaults.Add(pluginType, o);
            }

            return o;
        }

        public object GetObject(Type pluginType, Instance instance, ILifecycle lifecycle)
        {
            if (lifecycle is UniquePerRequestLifecycle)
            {
                return _resolver.BuildNewInSession(pluginType, instance);
            }

            var key = instance.InstanceKey(pluginType);
            if (!_cachedObjects.ContainsKey(key))
            {
                var o = _resolver.ResolveFromLifecycle(pluginType, instance);
                _cachedObjects[key] = o;

                return o;
            }

            return _cachedObjects[key];
        }

        public object TryGetDefault(Type pluginType, IPipelineGraph pipelineGraph)
        {
            if (_defaults.ContainsKey(pluginType)) return _defaults[pluginType];

            var instance = pipelineGraph.Instances.GetDefault(pluginType);
            if (instance == null) return null;

            var lifecycle = pipelineGraph.DetermineLifecycle(pluginType, instance);
            var o = GetObject(pluginType, instance, lifecycle);
            _defaults.Add(pluginType, o);

            return o;
        }

        // Tested through BuildSession
        public IEnumerable<T> All<T>()
        {
            return _cachedObjects.Values.OfType<T>();
        }
    }
}