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

        protected override LambdaInstance<T> thisInstance { get { return this; } }

        protected override object build(Type pluginType, BuildSession session)
        {
            try
            {
                return _builder(session);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, Name, pluginType);
            }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }

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