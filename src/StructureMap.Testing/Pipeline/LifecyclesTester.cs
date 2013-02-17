using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LifecyclesTester
    {
        [Test]
        public void getting_transient_gets_the_flyweight()
        {
            Lifecycles.GetLifecycle(InstanceScope.Transient).ShouldBeTheSameAs(Lifecycles.Transient);
        }

        [Test]
        public void getting_singleton_gets_the_flyweight()
        {
            Lifecycles.GetLifecycle(InstanceScope.Singleton).ShouldBeTheSameAs(Lifecycles.Singleton);
        }

        [Test]
        public void getting_unique_gets_the_flyweight()
        {
            Lifecycles.GetLifecycle(InstanceScope.Unique).ShouldBeTheSameAs(Lifecycles.Unique);
        }
    }
}