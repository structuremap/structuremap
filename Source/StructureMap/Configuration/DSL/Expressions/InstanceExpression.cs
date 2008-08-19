using System;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    public interface IsExpression<T>
    {
        InstanceExpression<T> Is { get; }
    }

    public class InstanceExpression<T> : IsExpression<T>
    {
        private readonly Action<Instance> _action;

        internal InstanceExpression(Action<Instance> action)
        {
            _action = action;
        }

        public void Is(Instance instance)
        {
            _action(instance);
        }

        private T returnInstance<T>(T instance) where T : Instance
        {
            Is(instance);
            return instance;
        }

        public SmartInstance<PLUGGEDTYPE> OfConcreteType<PLUGGEDTYPE>() where PLUGGEDTYPE : T
        {
            return returnInstance(new SmartInstance<PLUGGEDTYPE>());
        }

        public LiteralInstance Object(T theObject)
        {
            return returnInstance(new LiteralInstance(theObject));
        }

        InstanceExpression<T> IsExpression<T>.Is
        {
            get { return this; }
        }
    }
}
