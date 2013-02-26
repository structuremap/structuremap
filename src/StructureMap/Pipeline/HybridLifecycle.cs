namespace StructureMap.Pipeline
{
    public class HybridLifecycle : HttpLifecycleBase<HttpContextLifecycle, ThreadLocalStorageLifecycle>
    {

    }
}