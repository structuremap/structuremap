using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing
{
    [TestFixture]
    public class end_to_end_testing
    {
        [Test]
        public void build_a_full_application()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                runtime.Factory.Get<IJsonWriter>().ShouldNotBeNull();
            }
        }
    }
}