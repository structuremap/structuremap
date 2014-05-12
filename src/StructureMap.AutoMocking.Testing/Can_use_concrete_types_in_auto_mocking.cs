using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Testing;

namespace StructureMap.AutoMocking.Testing
{
    [TestFixture]
    public class Can_use_concrete_types_in_auto_mocking
    {
        [Test]
        public void use_mock_for_requested_concrete_type()
        {
            var mocker = new RhinoAutoMocker<ConcreteGuyUser>();

            mocker.Get<ConcreteGuy>().Stub(x => x.Color()).Return("Blue");

            mocker.ClassUnderTest.Guy.Color().ShouldEqual("Blue");
        }
    }

    public class ConcreteGuy
    {
        public virtual string Color()
        {
            return "Green";
        }
    }

    public class ConcreteGuyUser
    {
        private readonly ConcreteGuy _guy;

        public ConcreteGuyUser(ConcreteGuy guy)
        {
            _guy = guy;
        }

        public ConcreteGuy Guy
        {
            get { return _guy; }
        }
    }
}