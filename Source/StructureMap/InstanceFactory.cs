using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using StructureMap.Emitting;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap
{
    /// <summary>
    /// Default implementation of IInstanceFactory
    /// </summary>
    public class InstanceFactory : IInstanceFactory
    {
        private Type _pluginType;
        private string _assemblyName;
        private HybridDictionary _instanceBuilders;
        private MementoSource _source;

        #region constructor functions

        private InstanceFactory()
        {
            _instanceBuilders = new HybridDictionary();
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
                determineMementoSource(family);
                setPluginType(family.PluginType);
                processPlugins(family.Plugins);
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


        private void setPluginType(Type PluginType)
        {
            _pluginType = PluginType;
            _assemblyName = Guid.NewGuid().ToString().Replace(".", "") + "InstanceBuilderAssembly";
        }

        #endregion

        /// <summary>
        /// Links the child InstanceBuilder members to the parent InstanceManager
        /// </summary>
        /// <param name="Manager"></param>
        public void SetInstanceManager(InstanceManager Manager)
        {
            foreach (InstanceBuilder builder in _instanceBuilders.Values)
            {
                builder.SetInstanceManager(Manager);
            }
        }

        #region create instance builders

        private void processPlugins(PluginCollection plugins)
        {
            _instanceBuilders.Clear();

            Assembly assembly = createInstanceBuilderAssembly(plugins);

            foreach (Plugin plugin in plugins)
            {
                addPlugin(assembly, plugin);
            }
        }

        private Assembly createInstanceBuilderAssembly(PluginCollection plugins)
        {
            InstanceBuilderAssembly builderAssembly = new InstanceBuilderAssembly(_assemblyName, PluginType);

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

        /// <summary>
        /// The CLR System.Type that the InstanceFactory creates
        /// </summary>
        public Type PluginType
        {
            get { return _pluginType; }
        }


        private InstanceMemento findMemento(string instanceKey)
        {
            InstanceMemento memento = null;

            memento = _source.GetMemento(instanceKey);

            if (memento == null)
            {
                throw new StructureMapException(200, instanceKey, _pluginType.FullName);
            }

            return memento;
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
            return buildFromInstanceBuilder(resolvedMemento);
        }


        private object buildFromInstanceBuilder(InstanceMemento memento)
        {
            if (!_instanceBuilders.Contains(memento.ConcreteKey))
            {
                throw new StructureMapException(
                    201, memento.ConcreteKey, memento.InstanceKey, PluginType.FullName);
            }

            InstanceBuilder builder = (InstanceBuilder) _instanceBuilders[memento.ConcreteKey];

            try
            {
                return builder.BuildInstance(memento);
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
        }

        /// <summary>
        /// Retrieves the MementoSource member of the InstanceFactory
        /// </summary>
        public MementoSource Source
        {
            get { return _source; }
            set { _source = value; }
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
    }
}