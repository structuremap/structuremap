using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class PrototypeInstance : ExpressedInstance<PrototypeInstance>
    {
        private readonly ICloneable _prototype;


        public PrototypeInstance(ICloneable prototype)
        {
            _prototype = prototype;
        }

        protected override PrototypeInstance thisInstance
        {
            get { return this; }
        }


        protected override object build(Type pluginType, BuildSession session)
        {
            return _prototype.Clone();
        }


        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
            return _prototype.GetType().CanBeCastTo(family.PluginType);
        }

        protected override string getDescription()
        {
            return "Prototype of " + _prototype;
        }
    }
}