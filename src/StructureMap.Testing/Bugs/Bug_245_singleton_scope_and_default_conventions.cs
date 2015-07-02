using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_245_singleton_scope_and_default_conventions
    {
        [Test]
        public void can_set_lifecycle_with_default_conventions()
        {
            var container = new Container();
            container.Configure(o =>
            {
                o.Scan(x =>
                {
                    x.AssemblyContainingType<IMyFactory>();
                    x.Include(t => t == typeof (MyFactory));
                    x.WithDefaultConventions(); // Remove this to get Singleton() to work.
                });

                //o.For<IMyFactory>().Use<MyFactory>().SetLifecycleTo(Lifecycles.Singleton);
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