using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class ChildContainer_Singleton_error_330
    {
        [Test]
        public void should_be_able_to_instantiate_root()
        {
            var parentContainer = new Container();

            var childContainer = parentContainer.CreateChildContainer();

            childContainer.Configure(x =>
            {
                x.ForSingletonOf<IRoot>().Use<Root>();
                x.For<IDependency>().Use<Dependency>();
            });

            var dependency = childContainer.GetInstance<IDependency>(); // Works

            // Fixed
            childContainer.GetInstance<IRoot>().ShouldNotBeNull(); // Fails


            childContainer.Model.For<IRoot>().Lifecycle.ShouldBeOfType<ContainerLifecycle>();
        }

        // SAMPLE: singletons_to_child_container_are_isolated
        [Test]
        public void singletons_to_child_container_are_isolated()
        {
            var parentContainer = new Container(_ =>
            {
                _.For<IDependency>().Use<Dependency>();
            });

            var child1 = parentContainer.CreateChildContainer();
            child1.Configure(x =>
            {
                x.ForSingletonOf<IRoot>().Use<Root>();
            });

            var child2 = parentContainer.CreateChildContainer();
            child2.Configure(x =>
            {
                x.ForSingletonOf<IRoot>().Use<Root>();
            });

            // IRoot is a "singleton" within child1 usage
            child1.GetInstance<IRoot>().ShouldBeSameAs(child1.GetInstance<IRoot>());

            // IRoot is a "singleton" within child2 usage
            child2.GetInstance<IRoot>().ShouldBeSameAs(child2.GetInstance<IRoot>());

            // but, child1 and child2 both have a different IRoot
            child1.GetInstance<IRoot>()
                .ShouldNotBeTheSameAs(child2);
        }
        // ENDSAMPLE

        public interface IRoot
        {
        }

        public class Root : IRoot
        {
            public Root(IDependency dependency)
            {
            }
        }

        public interface IDependency
        {
        }

        public class Dependency : IDependency
        {
        }
    }
}