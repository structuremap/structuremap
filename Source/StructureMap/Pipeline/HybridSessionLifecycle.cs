using System;

namespace StructureMap.Pipeline
{
    public class HybridSessionLifecycle : HttpLifecycleBase<HttpSessionLifecycle, ThreadLocalStorageLifecycle>
    {
        public override string Scope
        {
            get { return InstanceScope.HybridHttpSession.ToString(); }
        }
    }
}