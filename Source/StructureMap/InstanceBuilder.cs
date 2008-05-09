using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Base class for creating an object instance from an InstanceMemento.  SubClasses are
    /// emitted for each concrete Plugin with constructor parameters.
    /// </summary>
    public abstract class InstanceBuilder
    {
        private InstanceManager _manager;

        public InstanceBuilder()
        {
        }

        public abstract string ConcreteTypeKey { get; }

        public abstract Type PluggedType { get; }

        public abstract object BuildInstance(IConfiguredInstance instance, StructureMap.Pipeline.IInstanceCreator creator);
    }
}