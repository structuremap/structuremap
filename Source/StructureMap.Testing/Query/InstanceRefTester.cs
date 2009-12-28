using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Query
{
    [TestFixture]
    public class InstanceRefTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            instance = new NullInstance();
            family = MockRepository.GenerateMock<IFamily>();

            instanceRef = new InstanceRef(instance, family);
        }

        #endregion

        private NullInstance instance;
        private IFamily family;
        private InstanceRef instanceRef;

        [Test]
        public void eject_object_calls_to_the_family()
        {
            instanceRef.EjectObject();

            family.AssertWasCalled(x => x.Eject(instance));
        }

        [Test]
        public void get_uses_the_family_to_return()
        {
            var widget = new AWidget();

            family.Stub(x => x.Build(instance)).Return(widget);

            instanceRef.Get<IWidget>().ShouldBeTheSameAs(widget);
        }

        [Test]
        public void has_relays_from_IFamily()
        {
            family.Stub(x => x.HasBeenCreated(instance)).Return(true);

            instanceRef.ObjectHasBeenCreated().ShouldBeTrue();
        }


        [Test]
        public void has_relays_from_IFamily_2()
        {
            family.Stub(x => x.HasBeenCreated(instance)).Return(false);

            instanceRef.ObjectHasBeenCreated().ShouldBeFalse();
        }

        [Test]
        public void name_just_relays()
        {
            instanceRef.Name.ShouldEqual(instance.Name);
        }

        [Test]
        public void plugin_type_comes_from_family()
        {
            family.Stub(x => x.PluginType).Return(typeof (IWidget));

            instanceRef.PluginType.ShouldEqual(typeof (IWidget));
        }
    }
}