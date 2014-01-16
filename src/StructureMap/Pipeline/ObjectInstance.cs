using System;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class ObjectInstance : ExpressedInstance<ObjectInstance>, IDisposable
    {
        private object _object;

        public ObjectInstance(object anObject)
        {
            if (anObject == null)
            {
                throw new ArgumentNullException("anObject");
            }

            _object = anObject;
        }


        protected override ObjectInstance thisInstance
        {
            get { return this; }
        }

        public object Object
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