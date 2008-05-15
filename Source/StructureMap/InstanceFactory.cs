using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Default implementation of IInstanceFactory
    /// </summary>
    public class InstanceFactory : IInstanceFactory
    {
        private readonly InstanceBuilderList _instanceBuilders;
        private readonly Dictionary<string, Instance> _instances = new Dictionary<string, Instance>();
        private readonly Type _pluginType;
        private readonly IBuildPolicy _policy = new BuildPolicy();

        #region constructor functions

        /// <summary>
        /// Constructor to use when troubleshooting possible configuration issues.
        /// </summary>
        /// <param name="family"></param>
        public InstanceFactory(PluginFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException("family");
            }

            try
            {
                _policy = family.Policy;

                _pluginType = family.PluginType;
                _instanceBuilders = new InstanceBuilderList(family.PluginType, family.Plugins.All);

                foreach (Instance instance in family.GetAllInstances())
                {
                    AddInstance(instance);
                }
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapException(115, e, family.PluginType.AssemblyQualifiedName);
            }
        }

        public static InstanceFactory CreateFactoryForType(Type concreteType, ProfileManager profileManager)
        {
            PluginFamily family = new PluginFamily(concreteType);
            family.Seal();

            InstanceFactory factory = new InstanceFactory(family);

            Instance instance = family.GetDefaultInstance();
            if (instance != null)
            {
                profileManager.SetDefault(concreteType, instance);
            }

            return factory;
        }

        #endregion

        #region IInstanceFactory Members

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public InstanceBuilder FindBuilderByType(Type pluggedType)
        {
            return _instanceBuilders.FindByType(pluggedType);
        }

        public InstanceBuilder FindBuilderByConcreteKey(string concreteKey)
        {
            return _instanceBuilders.FindByConcreteKey(concreteKey);
        }

        public void AddInstance(Instance instance)
        {
            if (_instances.ContainsKey(instance.Name))
            {
                _instances[instance.Name] = instance;
            }
            else
            {
                _instances.Add(instance.Name, instance);
            }
        }


        public Instance AddType<T>()
        {
            InstanceBuilder builder = _instanceBuilders.FindByType(typeof (T));
            ConfiguredInstance instance = new ConfiguredInstance();
            instance.WithConcreteKey(builder.ConcreteTypeKey).WithName(builder.ConcreteTypeKey);

            AddInstance(instance);

            return instance;
        }

        public IList GetAllInstances(IBuildSession session)
        {
            IList list = new ArrayList();

            foreach (KeyValuePair<string, Instance> pair in _instances)
            {
                object instance = Build(session, pair.Value);
                list.Add(instance);
            }

            return list;
        }

        public object Build(IBuildSession session, Instance instance)
        {
            return _policy.Build(session, PluginType, instance);
        }

        public object Build(IBuildSession session, string instanceKey)
        {
            if (!_instances.ContainsKey(instanceKey))
            {
                throw new StructureMapException(200, instanceKey, _pluginType.FullName);
            }

            Instance instance = _instances[instanceKey];
            return Build(session, instance);
        }

        #endregion
    }
}