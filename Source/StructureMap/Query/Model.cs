using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Query
{
    public class Model : IModel
    {
        private readonly IEnumerable<IPluginTypeConfiguration> _pluginTypes;

        internal Model(IEnumerable<IPluginTypeConfiguration> pluginTypes)
        {
            _pluginTypes = pluginTypes;
        }

        #region IModel Members

        public bool HasDefaultImplementationFor(Type pluginType)
        {
            return findForFamily(pluginType, f => f.Default != null);
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
            return findForFamily(pluginType, f => f.Default == null ? null : f.Default.ConcreteType);
        }

        public IEnumerable<IPluginTypeConfiguration> PluginTypes
        {
            get
            {
                return _pluginTypes;
            }
        }

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
            return _pluginTypes.FirstOrDefault(x => x.PluginType == type) ?? new EmptyConfiguration(type);
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

        public IEnumerable<InstanceRef> AllInstances { get { return PluginTypes.SelectMany(x => x.Instances); } }

        private T findForFamily<T>(Type pluginType, Func<IPluginTypeConfiguration, T> func, T defaultValue)
        {
            IPluginTypeConfiguration family = PluginTypes.FirstOrDefault(x => x.PluginType == pluginType);
            return family == null ? defaultValue : func(family);
        }

        private T findForFamily<T>(Type pluginType, Func<IPluginTypeConfiguration, T> func)
        {
            return findForFamily(pluginType, func, default(T));
        }

        #endregion
    }
}