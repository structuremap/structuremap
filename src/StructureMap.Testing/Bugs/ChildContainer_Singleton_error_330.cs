using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;

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