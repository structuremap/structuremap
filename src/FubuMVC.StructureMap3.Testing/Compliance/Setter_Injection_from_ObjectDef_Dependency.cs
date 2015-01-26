using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class Setter_Injection_from_ObjectDef_Dependency
    {
        [Test]
        public void will_set_properties_that_are_explicitly_set_on_ObjectDef()
        {
            var node = Process.For<OutsideBehavior>();
            node.AddAfter(Process.For<InsideBehavior>());

            var def = node.As<IContainerModel>().ToObjectDef();

            var behavior = ContainerFacilitySource
                .BuildBehavior(new ServiceArguments(), def, x => { })
                .As<OutsideBehavior>();

            behavior.InsideBehavior.ShouldBeOfType<InsideBehavior>();
        }
    }

    public class OutsideBehavior : BasicBehavior
    {
        public OutsideBehavior() : base(PartialBehavior.Executes)
        {
        }
    }

    public class InsideBehavior : BasicBehavior
    {
        public InsideBehavior()
            : base(PartialBehavior.Executes)
        {
        }
    }
}