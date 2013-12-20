using System;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// GoF Memento representing an Object Instance
    /// </summary>
    public abstract class InstanceMemento
    {
        public const string EMPTY_STRING = "STRING.EMPTY";
        private string _instanceKey;


        public ConstructorInstance ToInstance(IPluginFactory factory, Type pluginType)
        {
            var plugin = factory.PluginFor(PluggedType());

            var instance = new ConstructorInstance(plugin);
            if (InstanceKey.IsNotEmpty())
            {
                instance.Name = InstanceKey;
            }

            var reader = new InstanceMementoPropertyReader(instance, this, factory);
            plugin.VisitArguments(reader);

            return instance;
        }

        protected abstract string PluggedType();


        protected abstract string innerConcreteKey { get; }

        /// <summary>
        /// The named key of the object instance represented by the InstanceMemento
        /// </summary>
        public string InstanceKey
        {
            get
            {
                if (String.IsNullOrEmpty(_instanceKey))
                {
                    return innerInstanceKey;
                }
                else
                {
                    return _instanceKey;
                }
            }
            set { _instanceKey = value; }
        }

        protected abstract string innerInstanceKey { get; }

        /// <summary>
        /// Template pattern property specifying whether the InstanceMemento is simply a reference
        /// to another named instance.  Useful for child objects.
        /// </summary>
        public abstract bool IsReference { get; }

        /// <summary>
        /// Template pattern property specifying the instance key that the InstanceMemento refers to
        /// </summary>
        public abstract string ReferenceKey { get; }


        /// <summary>
        /// Is the InstanceMemento a reference to the default instance of the Plugin type?
        /// </summary>
        public bool IsDefault { get { return (IsReference && ReferenceKey == String.Empty); } }


        /// <summary>
        /// Retrieves the named property value as a string
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetProperty(string Key)
        {
            string returnValue = "";

            try
            {
                returnValue = getPropertyValue(Key);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(205, ex, Key, InstanceKey);
            }

            if (String.IsNullOrEmpty(returnValue)) return null;
            if (returnValue.ToUpper() == EMPTY_STRING)
            {
                returnValue = String.Empty;
            }

            return returnValue;
        }

        /// <summary>
        /// Template method for implementation specific retrieval of the named property
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract string getPropertyValue(string key);


        /// <summary>
        /// Returns the named child InstanceMemento
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public InstanceMemento GetChildMemento(string key)
        {
            InstanceMemento returnValue = getChild(key);
            return returnValue;
        }

        public virtual Instance ReadChildInstance(string name, IPluginFactory graph, Type childType)
        {
            InstanceMemento child = GetChildMemento(name);
            return child == null ? null : child.ReadInstance(graph, childType);
        }

        /// <summary>
        /// Template method for implementation specific retrieval of the named property
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract InstanceMemento getChild(string key);


        /// <summary>
        /// This method is made public for testing.  It is not necessary for normal usage.
        /// </summary>
        /// <returns></returns>
        public abstract InstanceMemento[] GetChildrenArray(string key);




        public Instance ReadInstance(IPluginFactory pluginFactory, Type pluginType)
        {
            if (pluginType == null)
            {
                throw new ArgumentNullException("pluginType");
            }

            try
            {
                Instance instance = readInstance(pluginFactory, pluginType);
                instance.Name = InstanceKey;

                return instance;
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapException(260, e, InstanceKey, pluginType.FullName);
            }
        }

        [CLSCompliant(false)]
        protected virtual Instance readInstance(IPluginFactory pluginFactory, Type pluginType)
        {
            if (IsDefault)
            {
                return new DefaultInstance();
            }

            if (IsReference)
            {
                return new ReferencedInstance(ReferenceKey);
            }

            return ToInstance(pluginFactory, pluginType);
        }
    }
}