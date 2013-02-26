namespace StructureMap.Pipeline
{
    public class HybridSessionLifecycle : HttpLifecycleBase<HttpSessionLifecycle, ThreadLocalStorageLifecycle>
    {

    }
}