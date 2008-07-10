using System;
using System.Collections;
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
        private readonly InstanceBuilderList _instanceBuilders;

        private readonly Cache<string, Instance> _instances =
            new Cache<string, Instance>(delegate { return null; });

        private readonly Type _pluginType;
        private readonly IBuildPolicy _policy = new BuildPolicy();

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
                _instanceBuilders = new InstanceBuilderList(family.PluginType, family.Plugins.All);


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

        public void ForEachInstance(Action<Instance> action)
        {
            _instances.Each(action);
        }

        public void AddInstance(Instance instance)
        {
            _instances.Store(instance.Name, instance);
        }


        [Obsolete]
        public Instance AddType<T>()
        {
            InstanceBuilder builder = _instanceBuilders.FindByType(typeof (T));
            ConfiguredInstance instance =
                new ConfiguredInstance(typeof (T)).WithName(TypePath.GetAssemblyQualifiedName(typeof (T)));

            AddInstance(instance);

            return instance;
        }

        public IList GetAllInstances(IBuildSession session)
        {
            IList list = new ArrayList();

            _instances.Each(instance =>
            {
                object builtObject = Build(session, instance);
                list.Add(builtObject);
            });

            return list;
        }

        public object Build(IBuildSession session, Instance instance)
        {
            return _policy.Build(session, PluginType, instance);
        }

        public Instance FindInstance(string name)
        {
            return _instances.Retrieve(name);
        }

        #endregion

        public void ImportFrom(PluginFamily family)
        {
            _instanceBuilders.Add(family.Plugins);
            family.EachInstance(instance => _instances.Fill(instance.Name, instance));
        }

        public void AcceptVisitor(IPipelineGraphVisitor visitor, Instance defaultInstance)
        {
            visitor.PluginType(PluginType, defaultInstance, _policy);
            ForEachInstance(i => visitor.Instance(PluginType, i));
        }
    }
}