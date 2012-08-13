using System;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class InstanceRef
    {
        private readonly IFamily _family;
        private readonly Instance _instance;

        public InstanceRef(Instance instance, IFamily family)
        {
            _instance = instance;
            _family = family;
        }

        internal Instance Instance { get { return _instance; } }

        public string Name { get { return _instance.Name; } }

        /// <summary>
        /// The actual concrete type of this Instance.  Not every type of IInstance
        /// can determine the ConcreteType
        /// </summary>
        public Type ConcreteType { get { return _instance.ConcreteType; } }


        public string Description { get { return _instance.Description; } }
        public Type PluginType { get { return _family.PluginType; } }

        public void EjectObject()
        {
            _family.Eject(_instance);
        }

        public T Get<T>() where T : class
        {
            return _family.Build(_instance) as T;
        }

        public bool ObjectHasBeenCreated()
        {
            return _family.HasBeenCreated(_instance);
        }
    }
}