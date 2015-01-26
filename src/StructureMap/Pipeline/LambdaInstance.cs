using System;
using System.Linq.Expressions;
using StructureMap.Building;
using StructureMap.Diagnostics;

namespace StructureMap.Pipeline
{
    public interface LambdaInstance
    {
        Type ReturnedType { get; }
    }

    public class LambdaInstance<T> : LambdaInstance<T, T>
    {
        public LambdaInstance(Expression<Func<IContext, T>> builder) : base(builder)
        {
        }

        public LambdaInstance(Expression<Func<T>> func) : base(func)
        {
        }

        public LambdaInstance(string description, Func<IContext, T> builder) : base(description, builder)
        {
        }

        public LambdaInstance(string description, Func<T> builder) : base(description, builder)
        {
        }
    }

    public class LambdaInstance<T, TPluginType> : ExpressedInstance<LambdaInstance<T, TPluginType>, T, TPluginType>, IDependencySource, LambdaInstance where T : TPluginType
    {
        private readonly Expression _builder;
        private readonly string _description;

        public LambdaInstance(Expression<Func<IContext, T>> builder)
        {
            _builder = builder.ReplaceParameter(typeof (IContext), Parameters.Context).Body;

            _description = builder
                .ReplaceParameter(typeof (IContext), Expression.Parameter(typeof (IContext), "IContext"))
                .Body.ToString();
        }

        public LambdaInstance(Expression<Func<T>> func)
        {
            _description = func.Body.ToString();
            _builder = func.Body;
        }

        public LambdaInstance(string description, Func<IContext, T> builder)
        {
            _description = description;
            _builder = Expression.Invoke(Expression.Constant(builder), Parameters.Context);
        }

        public LambdaInstance(string description, Func<T> builder)
        {
            _description = description;
            _builder = Expression.Invoke(Expression.Constant(builder));
        }

        protected override LambdaInstance<T, TPluginType> thisInstance
        {
            get { return this; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return this;
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            return _builder;
        }

        public override Type ReturnedType
        {
            get { return typeof (T); }
        }

        void IDependencySource.AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Dependency(this);
        }

        public override string Description
        {
            get { return "Lambda: " + _description; }
        }

        public override Instance CloseType(Type[] types)
        {
            if (typeof (T) == typeof (object))
            {
                return this;
            }

            return null;
        }
    }
}