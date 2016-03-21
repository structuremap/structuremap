using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class LifecyclesTester
    {
        [Fact]
        public void getting_transient_gets_the_flyweight()
        {
            Lifecycles.Get<TransientLifecycle>().ShouldBeTheSameAs(Lifecycles.Transient);
        }

        [Fact]
        public void getting_singleton_gets_the_flyweight()
        {
            Lifecycles.Get<SingletonLifecycle>().ShouldBeTheSameAs(Lifecycles.Singleton);
        }

        [Fact]
        public void getting_unique_gets_the_flyweight()
        {
            Lifecycles.Get<UniquePerRequestLifecycle>().ShouldBeTheSameAs(Lifecycles.Unique);
        }
    }
}