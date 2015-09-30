using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Query
{
    public class Model : IModel
    {
        private readonly IPipelineGraph _graph;

        internal Model(IPipelineGraph graph, PluginGraph pluginGraph)
        {
            _graph = graph;
            PluginGraph = pluginGraph;
        }

        public IPipelineGraph Pipeline
        {
            get { return _graph; }
        }

        private IEnumerable<IPluginTypeConfiguration> pluginTypes
        {
            get
            {
                foreach (var family in _graph.Instances.UniqueFamilies())
                {
                    if (family.IsGenericTemplate)
                    {
                        yield return new GenericFamilyConfiguration(family, _graph);
                    }
                    else
                    {
                        yield return new ClosedPluginTypeConfiguration(family, _graph);
                    }
                }
            }
        }

        public bool HasDefaultImplementationFor(Type pluginType)
        {
            return _graph.Instances.HasDefaultForPluginType(pluginType);
        }

        public bool HasDefaultImplementationFor<T>()
        {
            return HasDefaultImplementationFor(typeof (T));
        }

        public Type DefaultTypeFor<T>()
        {
            return DefaultTypeFor(typeof (T));
        }

        public Type DefaultTypeFor(Type pluginType)
        {
            return findForFamily(pluginType, f => f.Default == null ? null : f.Default.ReturnedType);
        }

        public IEnumerable<IPluginTypeConfiguration> PluginTypes
        {
            get { return pluginTypes; }
        }

        public PluginGraph PluginGraph { get; private set; }

        /// <summary>
        /// Retrieves the configuration for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IPluginTypeConfiguration For<T>()
        {
            return For(typeof (T));
        }

        /// <summary>
        /// Retrieves the configuration for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IPluginTypeConfiguration For(Type type)
        {
            return pluginTypes.FirstOrDefault(x => x.PluginType == type) ?? new EmptyConfiguration(type);
        }

        /// <summary>
        /// Eject all objects, configuration, and Plugin Types matching this filter
        /// </summary>
        /// <param name="filter"></param>
        public void EjectAndRemoveTypes(Func<Type, bool> filter)
        {
            // first pass hits Plugin types
            EjectAndRemovePluginTypes(filter);

            // second pass to hit instances
            pluginTypes.Each(x => { x.EjectAndRemove(i => filter(i.ReturnedType)); });
        }

        /// <summary>
        /// Eject all objects and configuration for any Plugin Type that matches this filter
        /// </summary>
        /// <param name="filter"></param>
        public void EjectAndRemovePluginTypes(Func<Type, bool> filter)
        {
            _graph.Ejector.RemoveCompletely(filter);
        }

        /// <summary>
        /// Eject all objects and Instance configuration for this PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        public void EjectAndRemove(Type pluginType)
        {
            _graph.Ejector.RemoveCompletely(pluginType);
        }

        public void EjectAndRemove<TPluginType>()
        {
            EjectAndRemove(typeof (TPluginType));
        }

        /// <summary>
        /// Get each and every configured instance that could possibly
        /// be cast to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllPossible<T>() where T : class
        {
            var targetType = typeof (T);
            return AllInstances
                .Where(x => x.ReturnedType.CanBeCastTo(targetType))
                .Select(x => x.Get<T>())
                .Where(x => x != null).ToArray();
        }

        public InstanceRef Find<TPluginType>(string name)
        {
            return For<TPluginType>().Find(name);
        }

        public IEnumerable<AssemblyScanner> Scanners
        {
            get { return PluginGraph.Registries.SelectMany(x => x.Scanners).ToArray(); }
        }

        public IEnumerable<InstanceRef> InstancesOf(Type pluginType)
        {
            return findForFamily(pluginType, x => x.Instances, new InstanceRef[0]);
        }

        public IEnumerable<InstanceRef> InstancesOf<T>()
        {
            return InstancesOf(typeof (T));
        }

        public bool HasImplementationsFor(Type pluginType)
        {
            return findForFamily(pluginType, x => x.HasImplementations());
        }

        public bool HasImplementationsFor<T>()
        {
            return HasImplementationsFor(typeof (T));
        }

        public IEnumerable<InstanceRef> AllInstances
        {
            get { return PluginTypes.ToList().SelectMany(x => x.Instances).ToList(); }
        }

        private T findForFamily<T>(Type pluginType, Func<IPluginTypeConfiguration, T> func, T defaultValue)
        {
            var family = PluginTypes.FirstOrDefault(x => x.PluginType == pluginType);
            return family == null ? defaultValue : func(family);
        }

        private T findForFamily<T>(Type pluginType, Func<IPluginTypeConfiguration, T> func)
        {
            return findForFamily(pluginType, func, default(T));
        }
    }
}