using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using StructureMap.Graph;
using StructureMap.Interceptors;
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
        private readonly InstanceInterceptor _interceptor = new NulloInterceptor();
        private readonly Type _pluginType;
        private InstanceManager _manager = new InstanceManager();
        private readonly IBuildPolicy _policy = new BuildPolicy();

        #region constructor functions

        /// <summary>
        /// Constructor to use when troubleshooting possible configuration issues.
        /// </summary>
        /// <param name="family"></param>
        /// <param name="failOnException">Toggles error trapping.  Set to "true" for diagnostics</param>
        public InstanceFactory(PluginFamily family)
        {
            if (family == null)
            {
                throw new NoNullAllowedException("'family' cannot be null");
            }

            try
            {
                _interceptor = family.InstanceInterceptor;
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

        private InstanceFactory(Type concreteType, ProfileManager profileManager)
        {
            _interceptor = new NulloInterceptor();
            _pluginType = concreteType;
            _instanceBuilders = new InstanceBuilderList(_pluginType, new Plugin[0]);

            if (_pluginType.IsAbstract || _pluginType.IsInterface)
            {
                return;
            }

            Plugin plugin = new Plugin(concreteType, Guid.NewGuid().ToString());
            if (plugin.CanBeAutoFilled)
            {
                _instanceBuilders = new InstanceBuilderList(_pluginType, new Plugin[] {plugin});

                ConfiguredInstance instance = new ConfiguredInstance();
                instance.PluggedType = concreteType;
                instance.Name = concreteType.FullName;

                _instances.Add(instance.Name, instance);

                profileManager.SetDefault(concreteType, instance);
            }
        }

        public static InstanceFactory CreateFactoryForConcreteType(Type concreteType, ProfileManager profileManager)
        {
            return new InstanceFactory(concreteType, profileManager);
        }


        #endregion

        #region IInstanceFactory Members

        /// <summary>
        /// Links the child InstanceBuilder members to the parent InstanceManager
        /// </summary>
        /// <param name="instanceManager"></param>
        public void SetInstanceManager(InstanceManager instanceManager)
        {
            _manager = instanceManager;
        }

        /// <summary>
        /// The CLR System.Type that the InstanceFactory creates
        /// </summary>
        public Type PluginType
        {
            get { return _pluginType; }
        }


        /// <summary>
        /// Creates an object instance for the named InstanceKey
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object GetInstance(string instanceKey)
        {
            if (!_instances.ContainsKey(instanceKey))
            {
                throw new StructureMapException(200, instanceKey, _pluginType.FullName);
            }

            Instance instance = _instances[instanceKey];

            return Build(instance);
        }

        public object Build(Instance instance)
        {
            return _policy.Build(_manager, PluginType, instance);
        }


        /// <summary>
        /// Creates an object instance for the supplied InstanceMemento
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetInstance(IConfiguredInstance instance, IInstanceCreator instanceCreator)
        {
            InstanceBuilder builder = instance.FindBuilder(_instanceBuilders);
            if (builder == null)
            {
                throw new StructureMapException(
                    201, instance.ConcreteKey, instance.Name, PluginType.FullName);
            }

            try
            {
                return builder.BuildInstance(instance, instanceCreator);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (InvalidCastException ex)
            {
                throw new StructureMapException(206, ex, instance.Name);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, instance.Name, PluginType.FullName);
            }
        }

        public IList GetAllInstances()
        {
            IList list = new ArrayList();

            foreach (KeyValuePair<string, Instance> pair in _instances)
            {
                object instance = _policy.Build(_manager, PluginType, pair.Value);
                list.Add(instance);
            }

            return list;
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
            instance.ConcreteKey = builder.ConcreteTypeKey;
            instance.Name = instance.ConcreteKey;


            return instance;
        }

        public object ApplyInterception(object rawValue)
        {
            return _interceptor.Process(rawValue);
        }



        #endregion
    }
}