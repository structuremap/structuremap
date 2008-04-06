using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using StructureMap.Configuration.Mementos;
using StructureMap.Emitting;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;

namespace StructureMap
{
    /// <summary>
    /// Default implementation of IInstanceFactory
    /// </summary>
    public class InstanceFactory : IInstanceFactory, IInstanceCreator
    {
        private readonly Dictionary<string, InstanceBuilder> _instanceBuilders;
        private readonly InstanceInterceptor _interceptor = new NulloInterceptor();
        private readonly Type _pluginType;
        private InterceptorLibrary _interceptorLibrary = InterceptorLibrary.Empty;
        private MementoSource _source;

        #region static constructors

        public static InstanceFactory CreateFactoryWithDefault(Type pluginType, object defaultInstance)
        {
            PluginFamily family = new PluginFamily(pluginType);
            InstanceFactory factory = new InstanceFactory(family, true);
            factory.SetDefault(new LiteralMemento(defaultInstance));

            return factory;
        }

        #endregion

        #region constructor functions

        private InstanceFactory()
        {
            _instanceBuilders = new Dictionary<string, InstanceBuilder>();
            _source = new MemoryMementoSource();
        }

        /// <summary>
        /// Constructor to use when troubleshooting possible configuration issues.
        /// </summary>
        /// <param name="family"></param>
        /// <param name="failOnException">Toggles error trapping.  Set to "true" for diagnostics</param>
        public InstanceFactory(PluginFamily family, bool failOnException) : this()
        {
            if (family == null)
            {
                throw new NoNullAllowedException("'family' cannot be null");
            }

            try
            {
                _interceptor = family.InstanceInterceptor;
                determineMementoSource(family);
                _pluginType = family.PluginType;
                processPlugins(family.Plugins.All);
                determineDefaultKey(family, failOnException);
            }
            catch (Exception e)
            {
                throw new StructureMapException(115, e, family.PluginTypeName);
            }
        }


        private void determineMementoSource(PluginFamily family)
        {
            if (family.Source == null)
            {
                _source = new MemoryMementoSource();
            }
            else
            {
                _source = family.Source;
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

        /// <summary>
        /// Retrieves the MementoSource member of the InstanceFactory
        /// </summary>
        public MementoSource Source
        {
            get { return _source; }
            set { _source = value; }
        }

        #region IInstanceCreator Members

        object IInstanceCreator.BuildInstance(InstanceMemento memento)
        {
            if (!_instanceBuilders.ContainsKey(memento.ConcreteKey))
            {
                throw new StructureMapException(
                    201, memento.ConcreteKey, memento.InstanceKey, PluginType.FullName);
            }


            try
            {
                InstanceBuilder builder = _instanceBuilders[memento.ConcreteKey];
                object constructedInstance = builder.BuildInstance(memento);
                CompoundInterceptor interceptor = _interceptorLibrary.FindInterceptor(constructedInstance.GetType());
                return interceptor.Process(constructedInstance);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (InvalidCastException ex)
            {
                throw new StructureMapException(206, ex, memento.InstanceKey);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, memento.InstanceKey, PluginType.FullName);
            }
        }

        InstanceMemento IInstanceCreator.DefaultMemento
        {
            get { return _source.DefaultMemento; }
        }

        #endregion

        #region IInstanceFactory Members

        /// <summary>
        /// Links the child InstanceBuilder members to the parent InstanceManager
        /// </summary>
        /// <param name="instanceManager"></param>
        public void SetInstanceManager(InstanceManager instanceManager)
        {
            _interceptorLibrary = instanceManager.InterceptorLibrary;
            foreach (InstanceBuilder builder in _instanceBuilders.Values)
            {
                builder.SetInstanceManager(instanceManager);
            }
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
            InstanceMemento memento = findMemento(instanceKey);
            return GetInstance(memento);
        }


        /// <summary>
        /// Creates an object instance for the supplied InstanceMemento
        /// </summary>
        /// <param name="memento"></param>
        /// <returns></returns>
        public object GetInstance(InstanceMemento memento)
        {
            // Let the MementoSource fill in Templates, resolve references, etc.
            InstanceMemento resolvedMemento = _source.ResolveMemento(memento);


            object instance = resolvedMemento.Build(this);
            return _interceptor.Process(instance);
        }


        /// <summary>
        /// Builds a new instance of the default instance of the PluginType
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            if (_source.DefaultMemento == null)
            {
                throw new StructureMapException(202, PluginType.FullName);
            }

            return GetInstance(_source.DefaultMemento);
        }


        /// <summary>
        /// Builds an array of object instances for every InstanceMemento passed in
        /// </summary>
        /// <param name="Mementos"></param>
        /// <returns></returns>
        public Array GetArray(InstanceMemento[] Mementos)
        {
            Array array = Array.CreateInstance(PluginType, Mementos.Length);
            for (int i = 0; i < Mementos.Length; i++)
            {
                object member = GetInstance(Mementos[i]);
                array.SetValue(member, i);
            }

            return array;
        }


        /// <summary>
        /// Sets the default InstanceMemento
        /// </summary>
        /// <param name="instanceKey"></param>
        public void SetDefault(string instanceKey)
        {
            if (instanceKey == string.Empty || instanceKey == null)
            {
                _source.DefaultMemento = null;
            }
            else
            {
                _source.DefaultMemento = findMemento(instanceKey);
            }
        }

        /// <summary>
        /// Sets the default InstanceMemento
        /// </summary>
        /// <param name="memento"></param>
        public void SetDefault(InstanceMemento memento)
        {
            _source.DefaultMemento = memento;
            if (_source.GetMemento(memento.InstanceKey) == null)
            {
                _source.AddExternalMemento(memento);
            }
        }

        /// <summary>
        /// The default instanceKey
        /// </summary>
        public string DefaultInstanceKey
        {
            get
            {
                InstanceMemento memento = _source.DefaultMemento;

                if (memento == null)
                {
                    return string.Empty;
                }
                else
                {
                    return memento.InstanceKey;
                }
            }
        }

        public IList GetAllInstances()
        {
            IList list = new ArrayList();

            foreach (InstanceMemento memento in _source.GetAllMementos())
            {
                object instance = GetInstance(memento);
                list.Add(instance);
            }

            return list;
        }

        public void AddInstance(InstanceMemento memento)
        {
            _source.AddExternalMemento(memento);
        }


        public InstanceMemento AddType<T>()
        {
            Type pluggedType = typeof (T);
            foreach (KeyValuePair<string, InstanceBuilder> pair in _instanceBuilders)
            {
                InstanceBuilder builder = pair.Value;
                if (builder.IsType(pluggedType))
                {
                    return new MemoryInstanceMemento(builder.ConcreteTypeKey, builder.ConcreteTypeKey);
                }
            }

            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (T));
            processPlugins(new Plugin[] {plugin});
            plugin.AddToSource(_source);

            return plugin.CreateImplicitMemento();
        }

        #region create instance builders

        private void processPlugins(IEnumerable<Plugin> plugins)
        {
            Assembly assembly = createInstanceBuilderAssembly(plugins);
            foreach (Plugin plugin in plugins)
            {
                addPlugin(assembly, plugin);
            }
        }

        private Assembly createInstanceBuilderAssembly(IEnumerable<Plugin> plugins)
        {
            string assemblyName = Guid.NewGuid().ToString().Replace(".", "") + "InstanceBuilderAssembly";
            InstanceBuilderAssembly builderAssembly = new InstanceBuilderAssembly(assemblyName, PluginType);

            foreach (Plugin plugin in plugins)
            {
                builderAssembly.AddPlugin(plugin);
            }

            return builderAssembly.Compile();
        }


        private void addPlugin(Assembly assembly, Plugin plugin)
        {
            string instanceBuilderClassName = plugin.GetInstanceBuilderClassName();
            InstanceBuilder builder = (InstanceBuilder) assembly.CreateInstance(instanceBuilderClassName);
            _instanceBuilders.Add(builder.ConcreteTypeKey, builder);
        }

        #endregion

        #endregion

        private InstanceMemento findMemento(string instanceKey)
        {
            InstanceMemento memento = _source.GetMemento(instanceKey);

            if (memento == null)
            {
                throw new StructureMapException(200, instanceKey, _pluginType.FullName);
            }

            return memento;
        }
    }
}