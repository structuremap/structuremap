using System;

namespace StructureMap.Pipeline
{
    public class PrototypeInstance : Instance
    {
        private ICloneable _prototype;


        public PrototypeInstance(ICloneable prototype)
        {
            _prototype = prototype;
        }


        protected override object build(Type type, IInstanceCreator creator)
        {
            // TODO:  VALIDATION IF IT CAN'T BE CAST
            return _prototype.Clone();
        }
    }
}