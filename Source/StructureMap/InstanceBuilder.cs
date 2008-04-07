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

        public abstract string PluginType { get; }
        public abstract string PluggedType { get; }
        public abstract string ConcreteTypeKey { get; }

        public abstract object BuildInstance(IConfiguredInstance instance, StructureMap.Pipeline.IInstanceCreator creator);

        public bool IsType(Type type)
        {
            Type plugged = Type.GetType(PluggedType);
            return plugged.Equals(type);
        }
    }
}