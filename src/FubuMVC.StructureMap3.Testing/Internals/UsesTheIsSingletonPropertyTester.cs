using FubuCore.Dates;
using FubuMVC.Core;
using NUnit.Framework;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Internals
{
    [TestFixture]
    public class UsesTheIsSingletonPropertyTester
    {
        [Test]
        public void IClock_should_be_a_singleton_just_by_usage_of_the_IsSingleton_property()
        {
            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            container.Model.For<IClock>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }
    }
}