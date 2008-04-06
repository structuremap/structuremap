using System;

namespace StructureMap.Pipeline
{
    public class LiteralInstance<PLUGINTYPE> : Instance
    {
        private readonly PLUGINTYPE _object;

        public LiteralInstance(PLUGINTYPE anObject)
        {
            _object = anObject;

            // TODO:  VALIDATE NOT NULL
        }


        protected override object build(Type type, IInstanceCreator creator)
        {
            // TODO:  VALIDATE THE CAST AND NULL

            return _object;
        }
    }
}