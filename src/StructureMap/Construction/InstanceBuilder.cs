using System;

namespace StructureMap.Construction
{
    public interface IInstanceBuilder
    {
        Type TPluggedType { get; }
        object BuildInstance(IArguments args);
        void BuildUp(IArguments args, object target);
    }

    /// <summary>
    /// Base class for creating an object instance from an InstanceMemento.  SubClasses are
    /// emitted for each concrete Plugin with constructor parameters.
    /// </summary>
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly Action<IArguments, object> _buildUp;
        private readonly Func<IArguments, object> _constructor;
        private readonly Type _TPluggedType;

        public InstanceBuilder(Type TPluggedType, Func<IArguments, object> constructor,
                               Action<IArguments, object> buildUp)
        {
            _TPluggedType = TPluggedType;
            _constructor = constructor;
            _buildUp = buildUp;
        }

        public Type TPluggedType { get { return _TPluggedType; } }

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