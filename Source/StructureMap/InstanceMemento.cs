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
        public const string SUBSTITUTIONS_ATTRIBUTE = "Substitutions";
        public const string TEMPLATE_ATTRIBUTE = "Template";
        private string _instanceKey;
        private string _lastKey = string.Empty;

        /// <summary>
        /// The named type of the object instance represented by the InstanceMemento.  Translates to a concrete
        /// type
        /// </summary>
        public string ConcreteKey
        {
            get { return innerConcreteKey; }
        }


        protected abstract string innerConcreteKey { get; }

        /// <summary>
        /// The named key of the object instance represented by the InstanceMemento
        /// </summary>
        public string InstanceKey
        {
            get
            {
                if (string.IsNullOrEmpty(_instanceKey))
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
        /// Gets the referred template name
        /// </summary>
        /// <returns></returns>
        public string TemplateName
        {
            get
            {
                string rawValue = getPropertyValue(TEMPLATE_ATTRIBUTE);
                return rawValue == null ? string.Empty : rawValue.Trim();
            }
        }

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
        /// Is the InstanceMemento a reference to the default instance of the plugin type?
        /// </summary>
        public bool IsDefault
        {
            get { return (IsReference && ReferenceKey == string.Empty); }
        }

        public virtual Plugin FindPlugin(PluginFamily family)
        {
            Plugin plugin = family.Plugins[innerConcreteKey] ?? getPluginByType(family) ??
                            family.Plugins[Plugin.DEFAULT];

            if (plugin == null)
            {
                throw new StructureMapException(201, innerConcreteKey, InstanceKey,
                                                family.PluginType.AssemblyQualifiedName);
            }

            return plugin;
        }

        private Plugin getPluginByType(PluginFamily family)
        {
            string pluggedTypeName = getPluggedType();
            if (string.IsNullOrEmpty(pluggedTypeName))
            {
                return null;
            }

            Type pluggedType = new TypePath(pluggedTypeName).FindType();

            return family.Plugins.FindOrCreate(pluggedType, false);
        }

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

            if (returnValue == string.Empty || returnValue == null)
            {
                throw new StructureMapException(205, Key, InstanceKey);
            }

            if (returnValue.ToUpper() == EMPTY_STRING)
            {
                returnValue = string.Empty;
            }

            return returnValue;
        }

        /// <summary>
        /// Template method for implementation specific retrieval of the named property
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        protected abstract string getPropertyValue(string Key);

        /// <summary>
        /// Returns the named child InstanceMemento
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public InstanceMemento GetChildMemento(string Key)
        {
            _lastKey = Key;

            InstanceMemento returnValue = getChild(Key);
            return returnValue;
        }

        /// <summary>
        /// Template method for implementation specific retrieval of the named property
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        protected abstract InstanceMemento getChild(string Key);


        /// <summary>
        /// This method is made public for testing.  It is not necessary for normal usage.
        /// </summary>
        /// <returns></returns>
        public abstract InstanceMemento[] GetChildrenArray(string Key);


        /// <summary>
        /// Used to create a templated InstanceMemento
        /// </summary>
        /// <param name="memento"></param>
        /// <returns></returns>
        public virtual InstanceMemento Substitute(InstanceMemento memento)
        {
            throw new NotSupportedException("This type of InstanceMemento does not support the Substitute() Method");
        }


        protected virtual string getPluggedType()
        {
            return getPropertyValue(XmlConstants.PLUGGED_TYPE);
        }

        public Instance ReadInstance(PluginGraph pluginGraph, Type pluginType)
        {
            try
            {
                Instance instance = readInstance(pluginGraph, pluginType);
                instance.Name = InstanceKey;
                instance.PluginType = pluginType;

                return instance;
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapException(260, InstanceKey, pluginType.FullName);
            }
        }

        protected virtual Instance readInstance(PluginGraph pluginGraph, Type pluginType)
        {
            if (IsDefault)
            {
                return new DefaultInstance();
            }

            if (IsReference)
            {
                return new ReferencedInstance(ReferenceKey);
            }

            return new ConfiguredInstance(this, pluginGraph, pluginType);
        }
    }
}