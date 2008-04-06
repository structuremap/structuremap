using System;

namespace StructureMap.Pipeline
{
    public class LiteralInstance<PLUGINTYPE> : Instance
    {
        private PLUGINTYPE _object;

        public LiteralInstance(PLUGINTYPE anObject)
        {
            _object = anObject;

            // TODO:  VALIDATE NOT NULL
        }

        protected override T build<T>(IInstanceCreator creator)
        {
            T returnValue = _object as T;
            // TODO:  VALIDATE THE CAST AND NULL

            return returnValue;
        }

        public override void Diagnose<T>(IInstanceCreator creator, IInstanceDiagnostics diagnostics)
        {
            throw new NotImplementedException();
        }

        public override void Describe<T>(IInstanceDiagnostics diagnostics)
        {
            throw new NotImplementedException();
        }
    }
}