using Moq;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Query
{
    public class InstanceRefTester
    {
        private readonly NullInstance instance;
        private readonly Mock<IFamily> familyMock;
        private readonly InstanceRef instanceRef;

        public InstanceRefTester()
        {
            instance = new NullInstance();
            familyMock = new Mock<IFamily>();

            instanceRef = new InstanceRef(instance, familyMock.Object);
        }

        [Fact]
        public void eject_object_calls_to_the_family()
        {
            instanceRef.EjectObject();

            familyMock.Verify(x => x.Eject(instance));
        }

        [Fact]
        public void get_uses_the_family_to_return()
        {
            var widget = new AWidget();

            familyMock.Setup(x => x.Build(instance)).Returns(widget);

            instanceRef.Get<IWidget>().ShouldBeTheSameAs(widget);
        }

        [Fact]
        public void has_relays_from_IFamily()
        {
            familyMock.Setup(x => x.HasBeenCreated(instance)).Returns(true);

            instanceRef.ObjectHasBeenCreated().ShouldBeTrue();
        }

        [Fact]
        public void has_relays_from_IFamily_2()
        {
            familyMock.Setup(x => x.HasBeenCreated(instance)).Returns(false);

            instanceRef.ObjectHasBeenCreated().ShouldBeFalse();
        }

        [Fact]
        public void name_just_relays()
        {
            instanceRef.Name.ShouldBe(instance.Name);
        }

        [Fact]
        public void plugin_type_comes_from_family()
        {
            familyMock.Setup(x => x.PluginType).Returns(typeof(IWidget));

            instanceRef.PluginType.ShouldBe(typeof(IWidget));
        }
    }
}