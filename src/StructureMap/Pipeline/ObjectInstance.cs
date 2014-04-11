using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class ObjectInstance : ObjectInstance<object, object>
    {
        public ObjectInstance(object anObject) : base(anObject)
        {
        }
    }

    public class ObjectInstance<TReturned, TPluginType> : ExpressedInstance<ObjectInstance<TReturned, TPluginType>, TReturned, TPluginType>, IDisposable where TReturned : class, TPluginType
    {
        private TReturned _object;

        public ObjectInstance(TReturned anObject)
        {
            if (null == anObject)
            {
                throw new ArgumentNullException("anObject");
            }

            _object = anObject;

            SetLifecycleTo<SingletonLifecycle>();
        }

        public override bool IsValidInNestedContainer()
        {
            return true;
        }

        protected override ObjectInstance<TReturned, TPluginType> thisInstance
        {
            get { return this; }
        }

        public TReturned Object
        {
            get { return _object; }
        }

        public void Dispose()
        {
            var isContainer = _object is IContainer;
            if (!isContainer)
            {
                _object.SafeDispose();
            }

            _object = null;
        }

        public override string Description
        {
            get { return "Object:  " + _object; }
        }

        public override string ToString()
        {
            return string.Format("LiteralInstance: {0}", _object);
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, _object);
        }

        public override Type ReturnedType
        {
            get { return _object.GetType(); }
        }
    }
}