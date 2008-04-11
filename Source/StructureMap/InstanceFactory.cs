using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap
{
    /// <summary>
    /// Default implementation of IInstanceFactory
    /// </summary>
    public class InstanceFactory : IInstanceFactory
    {
        private readonly InstanceBuilderList _instanceBuilders;
        private readonly InstanceInterceptor _interceptor = new NulloInterceptor();
        private readonly Type _pluginType;
        private InstanceManager _manager = new InstanceManager();
        private readonly Dictionary<string, Instance> _instances = new Dictionary<string, Instance>();
        private Instance _defaultInstance;

        #region static constructors

        public static InstanceFactory CreateFactoryWithDefault(Type pluginType, object defaultInstance)
        {
            PluginFamily family = new PluginFamily(pluginType);
            InstanceFactory factory = new InstanceFactory(family, true);

            LiteralInstance instance = new LiteralInstance(defaultInstance);
            factory.SetDefault(instance);

            return factory;
        }

        #endregion

        #region constructor functions

        /// <summary>
        /// Constructor to use when troubleshooting possible configuration issues.
        /// </summary>
        /// <param name="family"></param>
        /// <param name="failOnException">Toggles error trapping.  Set to "true" for diagnostics</param>
        public InstanceFactory(PluginFamily family, bool failOnException)
        {
            if (family == null)
            {
                throw new NoNullAllowedException("'family' cannot be null");
            }

            try
            {
                _interceptor = family.InstanceInterceptor;

                _pluginType = family.PluginType;
                _instanceBuilders = new InstanceBuilderList(family.PluginType, family.Plugins.All);

                foreach (Instance instance in family.GetAllInstances())
                {
                    AddInstance(instance);
                }

                determineDefaultKey(family, failOnException);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapException(115, e, family.PluginTypeName);
            }
        }

        public static InstanceFactory CreateInstanceFactoryForType(Type concreteType)
        {
            return new InstanceFactory(concreteType);
        }

        private InstanceFactory(Type concreteType)
        {
            _interceptor = new NulloInterceptor();
            _pluginType = concreteType;
            _instanceBuilders = new InstanceBuilderList(_pluginType, new Plugin[0]);

            if (_pluginType.IsAbstract || _pluginType.IsInterface)
            {
                return;
            }

            Plugin plugin = new Plugin(new TypePath(concreteType), Guid.NewGuid().ToString());
            if (plugin.CanBeAutoFilled)
            {
                _instanceBuilders = new InstanceBuilderList(_pluginType, new Plugin[]{plugin});

                ConfiguredInstance instance = new ConfiguredInstance();
                instance.PluggedType = concreteType;
                instance.Name = concreteType.FullName;

                _instances.Add(instance.Name, instance);

                _defaultInstance = instance;
            }
        }



        private void determineDefaultKey(PluginFamily family, bool failOnException)
        {
            if (family.DefaultInstanceKey != null && family.DefaultInstanceKey != string.Empty)
            {
                try
                {
                    SetDefault(family.DefaultInstanceKey);
                }
                catch (Exception)
                {
                    if (failOnException)
                    {
                        throw;
                    }
                }
            }
            else
            {
                setDefaultFromAttribute();
            }
        }

        private void setDefaultFromAttribute()
        {
            string defaultKey = PluginFamilyAttribute.GetDefaultKey(PluginType);
            if (defaultKey != string.Empty)
            {
                try
                {
                    SetDefault(defaultKey);
                }
                catch (Exception ex)
                {
                    throw new StructureMapException(10, ex, defaultKey, PluginType.FullName);
                }
            }
        }

        #endregion


        #region IInstanceCreator Members

        // TODO:  This code needs to move somewhere else
        //object IInstanceCreator.BuildInstance(InstanceMemento memento)
        //{
        //    InstanceBuilder builder = memento.FindBuilder(_instanceBuilders);

        //    if (builder == null)
        //    {
        //        throw new StructureMapException(
        //            201, memento.ConcreteKey, memento.InstanceKey, PluginType.FullName);
        //    }

        //    try
        //    {
        //        object constructedInstance = builder.BuildInstance(memento, _manager);
        //        InstanceInterceptor interceptor = _manager.FindInterceptor(constructedInstance.GetType());
        //        return interceptor.Process(constructedInstance);
        //    }
        //    catch (StructureMapException)
        //    {
        //        throw;
        //    }
        //    catch (InvalidCastException ex)
        //    {
        //        throw new StructureMapException(206, ex, memento.InstanceKey);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new StructureMapException(207, ex, memento.InstanceKey, PluginType.FullName);
        //    }
        //}

        //InstanceMemento IInstanceCreator.DefaultMemento
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //        //return _source.DefaultMemento;
        //    }
        //}

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

            return instance.Build(_pluginType, _manager);
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


        /// <summary>
        /// Builds a new instance of the default instance of the PluginType
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            if (_defaultInstance == null)
            {
                throw new StructureMapException(202, PluginType.FullName);
            }

            object builtObject = _defaultInstance.Build(_pluginType, _manager);
            return _interceptor.Process(builtObject);
        }


        /// <summary>
        /// Sets the default InstanceMemento
        /// </summary>
        /// <param name="instanceKey"></param>
        public void SetDefault(string instanceKey)
        {
            if (instanceKey == string.Empty || instanceKey == null)
            {
                _defaultInstance = null;
            }
            else
            {
                if (!_instances.ContainsKey(instanceKey))
                {
                    throw new StructureMapException(215, instanceKey, _pluginType.FullName);
                }

                _defaultInstance = _instances[instanceKey];
            }
        }

        /// <summary>
        /// Sets the default InstanceMemento
        /// </summary>
        /// <param name="instance"></param>
        public void SetDefault(Instance instance)
        {
            _defaultInstance = instance;
        }

        /// <summary>
        /// The default instanceKey
        /// </summary>
        public string DefaultInstanceKey
        {
            get
            {
                return _defaultInstance == null ? string.Empty : _defaultInstance.Name;
            }
        }

        public IList GetAllInstances()
        {
            IList list = new ArrayList();

            foreach (KeyValuePair<string, Instance> pair in _instances)
            {
                object instance = pair.Value.Build(_pluginType, _manager);
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

        public Instance GetDefault()
        {
            return _defaultInstance;
        }


        public object ApplyInterception(object rawValue)
        {
            return _interceptor.Process(rawValue);
        }

        #endregion

    }
}