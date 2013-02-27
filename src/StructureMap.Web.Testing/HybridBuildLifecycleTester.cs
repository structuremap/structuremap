using NUnit.Framework;
using StructureMap.Testing;
using StructureMap.Testing.DocumentationExamples;
using StructureMap.Testing.Widget3;

namespace StructureMap.Web.Testing
{
    [TestFixture]
    public class HybridBuildLifecycleTester
    {
        [Test]
        public void run_without_an_httpcontext()
        {
            var container = new Container(x => x.For<IService>(WebLifecycles.Hybrid).Use<RemoteService>());

            var object1 = container.GetInstance<IService>();
            var object2 = container.GetInstance<IService>();
            var object3 = container.GetInstance<IService>();

            object1.ShouldNotBeNull();
            object2.ShouldNotBeNull();
            object3.ShouldNotBeNull();

            object1.ShouldBeTheSameAs(object2).ShouldBeTheSameAs(object3);
        }
    }
}