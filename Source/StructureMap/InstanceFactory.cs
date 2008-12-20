using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly Cache<string, Instance> _instances =
            new Cache<string, Instance>(delegate { return null; });

        private readonly Type _pluginType;
        private IBuildPolicy _policy = new BuildPolicy();

        #region constructor functions

        public InstanceFactory(Type pluginType) : this(new PluginFamily(pluginType))
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
                _policy = family.Policy;

                _pluginType = family.PluginType;


                family.EachInstance(AddInstance);
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

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public IEnumerable<IInstance> Instances
        {
            get { return _instances.GetAll(); }
        }

        public IBuildPolicy Policy
        {
            get { return _policy; }
        }

        public void ForEachInstance(Action<Instance> action)
        {
            _instances.Each(action);
        }

        public void AddInstance(Instance instance)
        {
            _instances[instance.Name] = instance;
        }


        [Obsolete]
        public Instance AddType<T>()
        {
            ConfiguredInstance instance =
                new ConfiguredInstance(typeof (T)).WithName(TypePath.GetAssemblyQualifiedName(typeof (T)));

            AddInstance(instance);

            return instance;
        }

        public IList GetAllInstances(BuildSession session)
        {
            IList list = new ArrayList();

            _instances.Each(instance =>
            {
                object builtObject = Build(session, instance);
                list.Add(builtObject);
            });

            return list;
        }

        public object Build(BuildSession session, Instance instance)
        {
            return _policy.Build(session, PluginType, instance);
        }

        public Instance FindInstance(string name)
        {
            return _instances[name];
        }

        public void ImportFrom(PluginFamily family)
        {
            if (!_policy.GetType().Equals(family.Policy.GetType()))
            {
                // TODO:  Might need to clear out the existing policy when it's ejected
                _policy = family.Policy;
            }

            family.EachInstance(instance => _instances.Fill(instance.Name, instance));
        }

        public void EjectAllInstances()
        {
            _instances.Clear();
        }

        #endregion
    }
}