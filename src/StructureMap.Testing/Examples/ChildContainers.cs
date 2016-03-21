using Shouldly;
using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Examples
{
    public class ChildContainers
    {
        public class ChildSpecialService : IService { }

        // SAMPLE: show_a_child_container_in_action
        [Fact]
        public void show_a_child_container_in_action()
        {
            var parent = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();
                _.For<IService>().Use<AService>();
            });

            // Create a child container and override the
            // IService registration
            var child = parent.CreateChildContainer();
            child.Configure(_ =>
            {
                _.For<IService>().Use<ChildSpecialService>();
            });

            // The child container has a specific registration
            // for IService, so use that one
            child.GetInstance<IService>()
                .ShouldBeOfType<ChildSpecialService>();

            // The child container does not have any
            // override of IWidget, so it uses its parent's
            // configuration to resolve IWidget
            child.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();
        }

        // ENDSAMPLE

        // SAMPLE: nested_container_from_child
        [Fact]
        public void nested_container_from_child()
        {
            var parent = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();
                _.For<IService>().Use<AService>();
            });

            // Create a child container and override the
            // IService registration
            var child = parent.CreateChildContainer();
            child.Configure(_ =>
            {
                _.For<IService>().Use<ChildSpecialService>();
            });

            using (var nested = child.GetNestedContainer())
            {
                nested.GetInstance<IService>()
                    .ShouldBeOfType<ChildSpecialService>();
            }
        }

        // ENDSAMPLE

        // SAMPLE: stubs-with-child-containers
        public class TheRealService : IService { }

        public class StubbedService : IService { }

        [Fact]
        public void in_testing()
        {
            var container = new Container(_ =>
            {
                _.For<IService>().Use<TheRealService>();
            });

            // Create a new child container for only this test
            var testContainer = container.CreateChildContainer();

            // Override a service with a stub that's easier to control
            var stub = new StubbedService();
            testContainer.Inject<IService>(stub);

            // Now, continue with the test resolving application
            // services through the new child container....
        }

        // ENDSAMPLE
    }
}