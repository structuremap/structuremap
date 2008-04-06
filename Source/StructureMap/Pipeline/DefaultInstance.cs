using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override T build<T>(IInstanceCreator creator)
        {
            return creator.CreateInstance<T>();
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
