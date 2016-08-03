using System;
using System.Linq;
using StructureMap.Building;
using StructureMap.Diagnostics;

namespace StructureMap.Pipeline
{
    public interface IValue
    {
        object Value { get; }
    }

    public class LightweightObjectInstance : Instance, IBuildPlan
    {
        public object Object { get; set; }

        public LightweightObjectInstance(object @object)
        {
            Object = @object;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, Object);
        }

        public override string Description => Object.ToString();
        public object Build(IBuildSession session, IContext context)
        {
            return Object;
        }

        public override Type ReturnedType => Object.GetType();
        public void AcceptVisitor(IBuildPlanVisitor visitor)
        {
            visitor.InnerBuilder(new Constant(Object.GetType(), Object));
        }

        protected override IBuildPlan buildPlan(Type pluginType, Policies policies)
        {
            return this;
        }
    }

    public class ObjectInstance : ObjectInstance<object, object>
    {
        public ObjectInstance(object anObject) : base(anObject)
        {
        }
    }

    public class ObjectInstance<TReturned, TPluginType> : ExpressedInstance<ObjectInstance<TReturned, TPluginType>, TReturned, TPluginType>, IBuildPlan, IValue, IDisposable where TReturned : class, TPluginType
    {
        public ObjectInstance(TReturned anObject)
        {
            if (null == anObject)
            {
                throw new ArgumentNullException(nameof(anObject));
            }

            Object = anObject;

            SetLifecycleTo<ObjectLifecycle>();
        }

        public override Instance ToNamedClone(string name)
        {
            return new ObjectInstance<TReturned, TPluginType>(Object) {Name = name};
        }

        object IValue.Value => Object;

        protected override ObjectInstance<TReturned, TPluginType> thisInstance => this;

        public TReturned Object { get; private set; }

        public void Dispose()
        {
            var isContainer = Object is IContainer;
            if (!isContainer)
            {
                Object.SafeDispose();
            }

            Object = null;
        }

        public override string Description => "Object:  " + Object;

        public override string ToString()
        {
            return $"LiteralInstance: {Object}";
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, Object);
        }

        public override Type ReturnedType => Object.GetType();

        protected override IBuildPlan buildPlan(Type pluginType, Policies policies)
        {
            policies.Apply(pluginType, this);

            if (!Interceptors.Any())
            {
                return this;
            }

            return base.buildPlan(pluginType, policies);
        }

        void IBuildPlanVisitable.AcceptVisitor(IBuildPlanVisitor visitor)
        {
            visitor.InnerBuilder(new Constant(Object.GetType(), Object));
        }

        object IBuildPlan.Build(IBuildSession session, IContext context)
        {
            return Object;
        }
    }
}