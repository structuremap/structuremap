using System;
using System.Web.UI;

namespace StructureMap.Pipeline
{
    public class UserControlInstance : Instance
    {
        private readonly string _url;

        public UserControlInstance(string url)
        {
            _url = url;
        }

        protected override T build<T>(IInstanceCreator creator)
        {
            // TODO:  VALIDATION if it doesn't cast or can't be built
            return new Page().LoadControl(_url) as T;
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