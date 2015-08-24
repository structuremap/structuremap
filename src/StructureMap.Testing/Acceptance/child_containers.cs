using System;
using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class child_containers
    {
        private Container parent;
        private IContainer child;


        [SetUp]
        public void SetUp()
        {
            parent = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();
                _.For<IService>().Use<AService>();
            });

            child = parent.CreateChildContainer();
        }

        [Test]
        public void resolve_to_a_parent_transient_if_no_override()
        {
            child.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();
        }

        [Test]
        public void resolve_the_child_override()
        {
            child.Configure(x => { x.For<IWidget>().Use<BWidget>(); });

            child.GetInstance<IWidget>()
                .ShouldBeOfType<BWidget>();

            parent.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();
        }

        [Test]
        public void with_dependencies()
        {
            child.Configure(x => { x.For<IWidget>().Use<BWidget>(); });

            var guy = child.GetInstance<GuyWithWidgetAndService>();

            // Overridden in child
            guy.Widget.ShouldBeOfType<BWidget>();

            // Resolved from parent configuration
            guy.Service.ShouldBeOfType<AService>();
        }

        [Test]
        public void disposing_the_parent_container_also_disposes_child_containers()
        {
            child.Configure(x => x.ForSingletonOf<DisposableGuy>());
            var guy = child.GetInstance<DisposableGuy>();

            parent.Dispose();

            guy.WasDisposed.ShouldBeTrue();
        }

        [Test]
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