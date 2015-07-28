using System;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    /// <summary>
    /// GoF Memento representing an Object Instance
    /// </summary>
    public abstract class InstanceMemento
    {
        public const string EMPTY_STRING = "STRING.EMPTY";
        private string _instanceKey;

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
        public bool IsDefault
        {
            get { return (IsReference && ReferenceKey == String.Empty); }
        }


        /// <summary>
        /// Retrieves the named property value as a string
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetProperty(string Key)
        {
            var returnValue = "";

            try
            {
                returnValue = getPropertyValue(Key);
            }
            catch (Exception ex)
            {
                throw new StructureMapConfigurationException(
                    "Missing requested Instance property '{0}' for InstanceKey '{1}'".ToFormat(Key, InstanceKey), ex);
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
            var returnValue = getChild(key);
            return returnValue;
        }

        public virtual Instance ReadChildInstance(string name, Type childType)
        {
            throw new NotImplementedException();
//            InstanceMemento child = GetChildMemento(name);
//            return child == null ? null : child.ReadInstance(graph, childType);
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


        public Instance ReadInstance(Type pluginType)
        {
            if (pluginType == null)
            {
                throw new ArgumentNullException("pluginType");
            }

            try
            {
                var instance = readInstance(pluginType);
                instance.Name = InstanceKey;

                return instance;
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapConfigurationException(
                    "Malformed InstanceMemento {0} of PluginType {1}".ToFormat(InstanceKey, pluginType.GetFullName()), e);
            }
        }

        [CLSCompliant(false)]
        protected virtual Instance readInstance(Type pluginType)
        {
            if (IsDefault)
            {
                return new DefaultInstance();
            }

            if (IsReference)
            {
                return new ReferencedInstance(ReferenceKey);
            }

            throw new NotImplementedException();
        }
    }
}