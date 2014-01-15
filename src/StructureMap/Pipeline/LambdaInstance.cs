using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class LambdaInstance<T> : ExpressedInstance<LambdaInstance<T>>
    {
        private readonly Func<IContext, T> _builder;

        public LambdaInstance(Func<IContext, T> builder)
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
            throw new NotImplementedException("Need to redo this");
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

        protected override string getDescription()
        {
            return "Instance is created by Func<object> function:  " + _builder;
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