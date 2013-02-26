using StructureMap.Pipeline;

namespace StructureMap.Web.Pipeline
{
    public class HybridSessionLifecycle : HttpLifecycleBase<HttpSessionLifecycle, ThreadLocalStorageLifecycle>
    {

    }
}