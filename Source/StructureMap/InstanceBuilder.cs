using System;
using StructureMap.Pipeline;

namespace StructureMap
{
#pragma warning disable 169
    /// <summary>
    /// Base class for creating an object instance from an InstanceMemento.  SubClasses are
    /// emitted for each concrete Plugin with constructor parameters.
    /// </summary>
    public abstract class InstanceBuilder
    {
        private Container _manager;

        // DO NOT ELIMINATE THIS METHOD
        public InstanceBuilder(){}

        public abstract Type PluggedType { get; }

        public abstract object BuildInstance(IConfiguredInstance instance, BuildSession session);

        public virtual void BuildUp(IConfiguredInstance instance, BuildSession session, object target) { }
    }
#pragma warning restore 169
}