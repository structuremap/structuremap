using System.Collections;
using System.Collections.Specialized;
using StructureMap.Configuration;

namespace StructureMap
{
    /// <summary>
    /// An in-memory implementation of InstanceMemento.  
    /// </summary>
    public class MemoryInstanceMemento : InstanceMemento
    {
        private const string PluggedTypeKey = "PluggedType";

        #region statics

        /// <summary>
        /// Creates an instance of MemoryInstanceMemento that represents a reference to another
        /// instance.
        /// </summary>
        /// <param name="referenceKey">The referenced instance key to another instance</param>
        /// <returns></returns>
        public static MemoryInstanceMemento CreateReferencedInstanceMemento(string referenceKey)
        {
            var memento = new MemoryInstanceMemento();
            memento._referenceKey = referenceKey;
            memento._isReference = true;

            return memento;
        }

        /// <summary>
        /// Creates a MemoryInstanceMemento that represents a reference to the default instance
        /// of a plugin type.
        /// </summary>
        /// <returns></returns>
        public static MemoryInstanceMemento CreateDefaultInstanceMemento()
        {
            var memento = new MemoryInstanceMemento();
            memento._referenceKey = string.Empty;
            memento._isReference = true;

            return memento;
        }

        #endregion

        private readonly Hashtable _children = new Hashtable();
        private readonly NameValueCollection _properties = new NameValueCollection();
        private string _concreteKey;
        private string _instanceKey;
        private bool _isReference;
        private string _referenceKey;


        /// <summary>
        /// Constructs a MemoryInstanceMemento without properties
        /// </summary>
        /// <param name="concreteKey">The concrete key of the plugin type</param>
        /// <param name="instanceKey">The identifying instance key</param>
        public MemoryInstanceMemento(string concreteKey, string instanceKey)
            : this(concreteKey, instanceKey, new NameValueCollection())
        {
        }


        /// <summary>
        /// Constructs a MemoryInstanceMemento with properties
        /// </summary>
        /// <param name="concreteKey">The concrete key of the plugin type</param>
        /// <param name="instanceKey">The identifying instance key</param>
        /// <param name="properties">NameValueCollection of instance properties</param>
        public MemoryInstanceMemento(string concreteKey, string instanceKey, NameValueCollection properties)
        {
            _properties = properties;
            _concreteKey = concreteKey;
            _instanceKey = instanceKey;
        }


        public MemoryInstanceMemento()
        {
        }

        protected override string PluggedType()
        {
            return getPropertyValue(PluggedTypeKey);
        }

        /// <summary>
        /// See <cref>InstanceMemento</cref>
        /// </summary>
        protected override string innerConcreteKey { get { return _concreteKey; } }

        /// <summary>
        /// See <cref>InstanceMemento</cref>
        /// </summary>
        protected override string innerInstanceKey { get { return _instanceKey; } }

        /// <summary>
        /// See <cref>InstanceMemento</cref>
        /// </summary>
        public override bool IsReference { get { return _isReference; } }

        /// <summary>
        /// See <cref>InstanceMemento</cref>
        /// </summary>
        public override string ReferenceKey { get { return _referenceKey; } }

        public void SetConcreteKey(string concreteKey)
        {
            _concreteKey = concreteKey;
        }

        /// <summary>
        /// Sets the value of the named property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, string value)
        {
            string stringValue = value == string.Empty ? EMPTY_STRING : value;
            _properties[name] = stringValue;
        }

        /// <summary>
        /// Deletes a named property from the DefaultInstanceMemento
        /// </summary>
        /// <param name="Name"></param>
        public void RemoveProperty(string Name)
        {
            _properties.Remove(Name);
        }

        /// <summary>
        /// Links a child InstanceMemento as a named property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Memento"></param>
        public void AddChild(string name, InstanceMemento Memento)
        {
            _children.Add(name, Memento);
        }

        public void ReferenceChild(string name, string instanceKey)
        {
            InstanceMemento child = CreateReferencedInstanceMemento(instanceKey);
            AddChild(name, child);
        }


        /// <summary>
        /// Links an array of InstanceMemento's to a named array property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="childMementos"></param>
        public void AddChildArray(string name, InstanceMemento[] childMementos)
        {
            _children.Add(name, childMementos);
        }


        public void SetInstanceKey(string instanceKey)
        {
            _instanceKey = instanceKey;
        }

        protected override string getPropertyValue(string key)
        {
            return _properties[key];
        }

        protected override InstanceMemento getChild(string key)
        {
            return (InstanceMemento) _children[key];
        }


        protected bool hasProperty(string propertyName)
        {
            return _properties[propertyName] != null;
        }


        /// <summary>
        /// See <cref>InstanceMemento</cref>
        /// </summary>
        public override InstanceMemento[] GetChildrenArray(string key)
        {
            return (InstanceMemento[]) _children[key];
        }

        public void SetPluggedType<T>()
        {
            _properties[PluggedTypeKey] = typeof(T).AssemblyQualifiedName;
        }
    }
}