using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class child_containers
    {
        private readonly Container parent;
        private readonly IContainer child;

        public child_containers()
        {
            parent = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();
                _.For<IService>().Use<AService>();
            });

            child = parent.CreateChildContainer();
        }

        [Fact]
        public void resolve_to_a_parent_transient_if_no_override()
        {
            child.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();
        }

        [Fact]
        public void resolve_the_child_override()
        {
            child.Configure(x => { x.For<IWidget>().Use<BWidget>(); });

            child.GetInstance<IWidget>()
                .ShouldBeOfType<BWidget>();

            parent.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();
        }

        [Fact]
        public void with_dependencies()
        {
            child.Configure(x => { x.For<IWidget>().Use<BWidget>(); });

            var guy = child.GetInstance<GuyWithWidgetAndService>();

            // Overridden in child
            guy.Widget.ShouldBeOfType<BWidget>();

            // Resolved from parent configuration
            guy.Service.ShouldBeOfType<AService>();
        }

        [Fact]
        public void disposing_the_parent_container_also_disposes_child_containers()
        {
            child.Configure(x => x.ForSingletonOf<DisposableGuy>());
            var guy = child.GetInstance<DisposableGuy>();

            parent.Dispose();

            guy.WasDisposed.ShouldBeTrue();
        }

        [Fact]
        public void disposing_child_does_not_dispose_singletons_created_by_parent()
        {
            parent.Configure(x => x.ForSingletonOf<DisposableGuy>());

            child.Configure(x =>
            {
                x.For<DisposableGuy>().Use<DisposableGuy>().Singleton();
            });

            var guy1 = parent.GetInstance<DisposableGuy>();
            var guy2 = child.GetInstance<DisposableGuy>();

            child.Dispose();

            guy1.WasDisposed.ShouldBeFalse();
            guy2.WasDisposed.ShouldBeTrue();
        }

        [Fact]
        public void stress_test_creation_and_disposal_of_child_containers_in_concurrent_operations()
        {
            Parallel.ForEach(Enumerable.Range(0, 1000000), i =>
            {
                using (parent.CreateChildContainer()) { }
            });
        }

        public class DisposableGuy : IDisposable
        {
            public bool WasDisposed;

            public void Dispose()
            {
                WasDisposed = true;
            }
        }
    }
}