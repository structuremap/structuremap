using NUnit.Framework;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class InterceptionPolicies_with_child_containers
    {
        [Test]
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
    }
}