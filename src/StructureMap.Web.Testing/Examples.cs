using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Samples;
using StructureMap.Web.Pipeline;

namespace StructureMap.Web.Testing
{
    [TestFixture]
    public class Examples
    {
        public void cleanup()
        {
            // SAMPLE: clean-up-http-context
            HttpContextLifecycle.DisposeAndClearAll();
            // ENDSAMPLE
        }
    }

    // SAMPLE: AspNet-Lifecycles
    public class AspNetRegistry : Registry
    {
        public AspNetRegistry()
        {
            For<IWeirdThing>().LifecycleIs<HttpContextLifecycle>();
            For<IGateway>().LifecycleIs<HybridLifecycle>();
            For<IRule>().LifecycleIs<HttpSessionLifecycle>();
            For<ICache>().LifecycleIs<HybridSessionLifecycle>();
        }
    }
    // ENDSAMPLE

    public interface IRule { }

    public interface ICache { }
    public class Cache { }

}