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
            Lifecycles.Get<TransientLifecycle>().ShouldBeTheSameAs(Lifecycles.Transient);
        }

        [Test]
        public void getting_singleton_gets_the_flyweight()
        {
            Lifecycles.Get<SingletonLifecycle>().ShouldBeTheSameAs(Lifecycles.Singleton);
        }

        [Test]
        public void getting_unique_gets_the_flyweight()
        {
            Lifecycles.Get<UniquePerRequestLifecycle>().ShouldBeTheSameAs(Lifecycles.Unique);
        }
    }
}