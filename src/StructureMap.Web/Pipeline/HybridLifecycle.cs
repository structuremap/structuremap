using StructureMap.Pipeline;

namespace StructureMap.Web.Pipeline
{
    public class HybridLifecycle : HttpLifecycleBase<HttpContextLifecycle, ThreadLocalStorageLifecycle>
    {

    }
}