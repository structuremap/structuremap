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


        protected override T build<T>(IInstanceCreator creator)
        {
            // TODO:  VALIDATION IF IT CAN'T BE CAST
            return (T) _prototype.Clone();
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