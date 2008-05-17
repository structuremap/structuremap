using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class LiteralInstance : ExpressedInstance<LiteralInstance>
    {
        private readonly object _object;

        public LiteralInstance(object anObject)
        {
            if (anObject == null)
            {
                throw new ArgumentNullException("anObject");
            }

            _object = anObject;
        }


        protected override LiteralInstance thisInstance
        {
            get { return this; }
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            return _object;
        }


        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
            return TypeRules.CanBeCast(family.PluginType, _object.GetType());
        }

        protected override string getDescription()
        {
            return "Object:  " + _object.ToString();
        }
    }
}