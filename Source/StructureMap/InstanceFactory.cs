using System;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap
{
    /// <summary>
    /// Default implementation of IInstanceFactory
    /// </summary>
    public class InstanceFactory : IInstanceFactory
    {
        private readonly Type _pluginType;

        private Cache<string, Instance> _instances =
            new Cache<string, Instance>(delegate { return null; });

        private ILifecycle _lifecycle;

        #region constructor functions

        public InstanceFactory(Type pluginType)
            : this(new PluginFamily(pluginType))
        {
        }

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
                _lifecycle = family.Lifecycle;

                _pluginType = family.PluginType;
                MissingInstance = family.MissingInstance;

                family.Instances.Each(AddInstance);
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
            var family = new PluginFamily(concreteType);
            family.Seal();

            var factory = new InstanceFactory(family);

            Instance instance = family.GetDefaultInstance();
            if (instance != null)
            {
                profileManager.SetDefault(concreteType, instance);
            }

            return factory;
        }

        #endregion

        #region IInstanceFactory Members

        public Instance MissingInstance { get; set; }

        public Type PluginType { get { return _pluginType; } }

        public Instance[] AllInstances { get { return _instances.GetAll(); } }


        public void AddInstance(Instance instance)
        {
            _instances[instance.Name] = instance;
        }


        public Instance FindInstance(string name)
        {
            return _instances[name] ?? MissingInstance;
        }

        public void ImportFrom(PluginFamily family)
        {
            if (_lifecycle == null)
            {
                _lifecycle = family.Lifecycle;
            }
            else if (_lifecycle != null && family.Lifecycle != null &&
                     !_lifecycle.GetType().Equals(family.Lifecycle.GetType()))
            {
                // TODO:  Might need to clear out the existing policy when it's ejected
                _lifecycle = family.Lifecycle;
            }


            family.Instances.Each(instance => _instances.Fill(instance.Name, instance));

            if (family.MissingInstance != null)
            {
                MissingInstance = family.MissingInstance;
            }
        }

        public void EjectAllInstances()
        {
            if (_lifecycle != null) _lifecycle.EjectAll();
            _instances.Clear();
        }

        public ILifecycle Lifecycle { get { return _lifecycle; } set { _lifecycle = value; } }

        public IInstanceFactory Clone()
        {
            var factory = new InstanceFactory(_pluginType);

            factory.MissingInstance = MissingInstance;
            factory._lifecycle = _lifecycle;
            factory._instances = _instances.Clone();

            return factory;
        }

        #endregion

        public void Dispose()
        {
            _instances.Clear();
        }
    }
}