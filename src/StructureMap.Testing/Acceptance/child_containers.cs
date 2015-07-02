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
    }
}