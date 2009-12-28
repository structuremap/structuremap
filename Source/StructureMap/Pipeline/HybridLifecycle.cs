namespace StructureMap.Pipeline
{
    public class HybridLifecycle : HttpLifecycleBase<HttpContextLifecycle, ThreadLocalStorageLifecycle>
    {
        public override string Scope
        {
            get { return InstanceScope.Hybrid.ToString(); }
        }
    }
}