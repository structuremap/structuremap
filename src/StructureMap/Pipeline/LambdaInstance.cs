using System;
using System.Linq.Expressions;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class LambdaInstance<T> : ExpressedInstance<LambdaInstance<T>>, IDependencySource
    {
        private readonly Func<IBuildSession, T> _builder;

        public LambdaInstance(Func<IBuildSession, T> builder)
        {
            _builder = builder;
        }

        public LambdaInstance(Func<T> func)
        {
            _builder = s => func();
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
            return Expression.Invoke(Expression.Constant(_builder), Parameters.Session);
        }

        public override Type ReturnedType
        {
            get
            {
                return typeof (T);
            }
        }

//        protected override object build(Type pluginType, IBuildSession session)
//        {
//            try
//            {
//                return _builder(session);
//            }
//                // TODO -- UT this behavior
//            catch (StructureMapException ex)
//            {
//                ex.Push(Description);
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw new StructureMapBuildException("Exception while trying to build '{0}', check the inner exception".ToFormat(Description), ex);
//            }
//        }

        public override string Description
        {
            get { return "Instance is created by Func<object> function:  " + _builder; }
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