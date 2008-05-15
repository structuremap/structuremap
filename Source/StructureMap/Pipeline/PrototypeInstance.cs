using System;

namespace StructureMap.Pipeline
{
    public class PrototypeInstance : ExpressedInstance<PrototypeInstance>
    {
        private ICloneable _prototype;


        public PrototypeInstance(ICloneable prototype)
        {
            _prototype = prototype;
        }

        protected override PrototypeInstance thisInstance
        {
            get { return this; }
        }


        protected override object build(Type pluginType, IBuildSession session)
        {
            // TODO:  VALIDATION IF IT CAN'T BE CAST
            return _prototype.Clone();
        }
    }
}