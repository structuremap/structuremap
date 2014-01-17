using System;
using System.Linq.Expressions;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class LambdaInstance<T> : ExpressedInstance<LambdaInstance<T>>, IDependencySource
    {
        private readonly Expression _builder;
        private readonly string _description;

        public LambdaInstance(Expression<Func<IBuildSession, T>> builder)
        {
            _builder = builder.ReplaceParameter(typeof(IBuildSession), Parameters.Session).Body;

            _description = builder
                .ReplaceParameter(typeof (IBuildSession), Expression.Parameter(typeof (IBuildSession), "IBuildSession"))
                .Body.ToString();
        }

        public LambdaInstance(Expression<Func<T>> func)
        {
            _description = func.Body.ToString();
            _builder = func.Body;
        }

        public LambdaInstance(string description, Func<IBuildSession, T> builder)
        {
            _description = description;
            _builder = Expression.Invoke(Expression.Constant(builder), Parameters.Session);
        }

        public LambdaInstance(string description, Func<T> builder)
        {
            _description = description;
            _builder = Expression.Invoke(Expression.Constant(builder));
        } 

        protected override LambdaInstance<T> thisInstance
        {
            get { return this; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return this;
        }

        public Expression ToExpression(ParameterExpression session)
        {
            return _builder;
        }

        public override Type ReturnedType
        {
            get
            {
                return typeof (T);
            }
        }

        public override string Description
        {
            get { return _description; }
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