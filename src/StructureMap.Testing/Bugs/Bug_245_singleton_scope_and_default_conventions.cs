using Shouldly;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_245_singleton_scope_and_default_conventions
    {
        [Fact]
        public void can_set_lifecycle_with_default_conventions()
        {
            var container = new Container();
            container.Configure(o =>
            {
                o.Scan(x =>
                {
                    x.AssemblyContainingType<IMyFactory>();
                    x.Include(t => t == typeof(MyFactory));
                    x.WithDefaultConventions(); // Remove this to get SingletonThing() to work.
                });

                //o.For<IMyFactory>().Use<MyFactory>().SetLifecycleTo(Lifecycles.SingletonThing);
                o.For<IMyFactory>().Singleton().Use<MyFactory>();
            });

            container.Model.For<IMyFactory>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        public interface IMyFactory
        {
        }

        public class MyFactory : IMyFactory
        {
        }
    }
}