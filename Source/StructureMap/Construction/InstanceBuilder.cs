using System;
using StructureMap.Pipeline;

namespace StructureMap.Construction
{
    public interface IInstanceBuilder
    {
        Type PluggedType { get; }
        object BuildInstance(IArguments args);
        void BuildUp(IArguments args, object target);
    }

    /// <summary>
    /// Base class for creating an object instance from an InstanceMemento.  SubClasses are
    /// emitted for each concrete Plugin with constructor parameters.
    /// </summary>
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly Type _pluggedType;
        private readonly Func<IArguments, object> _constructor;
        private readonly Action<IArguments, object> _buildUp;

        public InstanceBuilder(Type pluggedType, Func<IArguments, object> constructor, Action<IArguments, object> buildUp)
        {
            _pluggedType = pluggedType;
            _constructor = constructor;
            _buildUp = buildUp;
        }

        public Type PluggedType { get
        {
            return _pluggedType;
        } 
        }

        public virtual object BuildInstance(IArguments args)
        {
            return _constructor(args);
        }

        public virtual void BuildUp(IArguments args, object target)
        {
            _buildUp(args, target);
        }
    }
}