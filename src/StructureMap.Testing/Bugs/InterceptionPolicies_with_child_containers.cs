using Shouldly;
using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class InterceptionPolicies_with_child_containers
    {
        [Fact]
        public void policies_should_apply_to_child_containers()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();
                x.For<Activateable>()
                    .OnCreationForAll("Mark the object as activated", o => o.Activated = true);
            });

            container.CreateChildContainer().GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>()
                .Activated.ShouldBeTrue();
        }

        [Fact]
        public void policies_should_apply_to_nested_containers()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();
                x.For<Activateable>()
                    .OnCreationForAll("Mark the object as activated", o => o.Activated = true);
            });

            container.GetNestedContainer().GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>()
                .Activated.ShouldBeTrue();
        }
    }
}