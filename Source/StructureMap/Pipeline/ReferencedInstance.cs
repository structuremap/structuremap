using System;

namespace StructureMap.Pipeline
{
    public class ReferencedInstance : Instance
    {
        private readonly string _referenceKey;


        public ReferencedInstance(string referenceKey)
        {
            // TODO:  VALIDATION if referenceKey is null or empty
            _referenceKey = referenceKey;
        }

        protected override T build<T>(IInstanceCreator creator)
        {
            return creator.CreateInstance<T>(_referenceKey);
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