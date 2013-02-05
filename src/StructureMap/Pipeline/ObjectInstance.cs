using System;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class NullInstance : Instance
    {
        protected override string getDescription()
        {
            return "NULL";
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            return null;
        }
    }

    public class ObjectInstance : ExpressedInstance<ObjectInstance>, IDisposable
    {
        private object _object;

        public ObjectInstance(object anObject)
        {
            CopyAsIsWhenClosingInstance = true;

            if (anObject == null)
            {
                throw new ArgumentNullException("anObject");
            }

            _object = anObject;
        }


        protected override ObjectInstance thisInstance { get { return this; } }

        public object Object { get { return _object; } }

        public void Dispose()
        {
            bool isContainer = _object is IContainer;
            if (!isContainer)
            {
                _object.SafeDispose();
            }

            _object = null;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            return _object;
        }


        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
            return _object.GetType().CanBeCastTo(family.PluginType);
        }

        protected override string getDescription()
        {
            return "Object:  " + _object;
        }

        public override string ToString()
        {
            return string.Format("LiteralInstance: {0}", _object);
        }

        protected override Type getConcreteType(Type pluginType)
        {
            return _object.GetType();
        }

        public ObjectInstance Named(string name)
        {
            Name = name;
            return this;
        }
    }
}