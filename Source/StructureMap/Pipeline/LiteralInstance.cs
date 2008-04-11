using System;

namespace StructureMap.Pipeline
{
    public class LiteralInstance : ExpressedInstance<LiteralInstance>
    {
        private readonly object _object;

        public LiteralInstance(object anObject)
        {
            _object = anObject;

            // TODO:  VALIDATE NOT NULL
        }


        protected override LiteralInstance thisInstance
        {
            get { return this; }
        }

        protected override object build(Type pluginType, IInstanceCreator creator)
        {
            // TODO:  VALIDATE THE CAST AND NULL

            return _object;
        }

    }
}