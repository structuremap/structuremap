using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class StubbedInstanceFactoryTester
    {
        private WidgetStub _stub;
        private StubbedInstanceFactory _stubbedFactory;

        [SetUp]
        public void SetUp()
        {
            PluginFamily family = ObjectMother.GetPluginFamily(typeof (IWidget));
            InstanceFactory factory = new InstanceFactory(family, true);

            _stub = new WidgetStub();
            _stubbedFactory = new StubbedInstanceFactory(factory, _stub);
        }


        [Test]
        public void InjectStubAndRetrieveStub()
        {
            object instance1 = _stubbedFactory.GetInstance("Red");
            object instance2 = _stubbedFactory.GetInstance();
            object instance3 = _stubbedFactory.GetInstance(new MemoryInstanceMemento("Color", "Orange"));

            Assert.AreSame(_stub, instance1);
            Assert.AreSame(_stub, instance2);
            Assert.AreSame(_stub, instance3);
        }

        [Test]
        public void IsMocked()
        {
            Assert.IsTrue(_stubbedFactory.IsMockedOrStubbed);
        }


        [Test]
        public void InjectStubSuccessful()
        {
            PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
            InstanceManager manager = new InstanceManager(pluginGraph);

            manager.InjectStub(typeof (IWidget), _stub);
            IWidget widget1 = (IWidget) manager.CreateInstance(typeof (IWidget));
            IWidget widget2 = (IWidget) manager.CreateInstance(typeof (IWidget), "Red");

            Assert.AreSame(_stub, widget1);
            Assert.AreSame(_stub, widget2);

            Assert.IsTrue(manager.IsMocked(typeof (IWidget)));
            manager.UnMock(typeof (IWidget));
            Assert.IsFalse(manager.IsMocked(typeof (IWidget)));
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  220\nCannot \"Stub\" type StructureMap.Testing.Widget.IWidget with an object of type System.String"
             )]
        public void InjectStubThatIsWrongType()
        {
            PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
            InstanceManager manager = new InstanceManager(pluginGraph);

            manager.InjectStub(typeof (IWidget), "A String is not an IWidget");
        }
    }


    public class WidgetStub : IWidget
    {
        public void DoSomething()
        {
        }
    }
}