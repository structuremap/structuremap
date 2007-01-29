using System;
using NMock;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class MockingTester
    {
        private InstanceManager _manager;
        private Type gatewayType = typeof (IGateway);

        [SetUp]
        public void SetUp()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add("StructureMap.Testing.Widget3");
            graph.PluginFamilies.Add(gatewayType, string.Empty, new MemoryMementoSource());
            graph.Seal();
            _manager = new InstanceManager(graph);
        }

        [Test]
        public void DoesIsMockedWork()
        {
            Assert.AreEqual(false, _manager.IsMocked(gatewayType), "Not mocked yet");

            _manager.Mock(gatewayType);

            Assert.AreEqual(true, _manager.IsMocked(gatewayType), "Now we're mocked");
        }

        [Test]
        public void GetTheMock()
        {
            Assert.AreEqual("Default", getGateway().WhoAmI, "WhoAmI = Default before mocking");

            IMock mock = _manager.Mock(gatewayType);
            mock.ExpectAndReturn("WhoAmI", "TheMock");

            Assert.AreEqual("TheMock", getGateway().WhoAmI, "WhoAmI = TheMock after mocking");
            mock.Verify();
        }

        [Test]
        public void UnMock()
        {
            _manager.Mock(gatewayType);
            Assert.AreEqual(true, _manager.IsMocked(gatewayType), "Now we're mocked");

            _manager.UnMock(gatewayType);
            Assert.AreEqual(false, _manager.IsMocked(gatewayType), "Not mocked anymore");

            Assert.AreEqual("Default", getGateway().WhoAmI, "WhoAmI = Default without mocking");
        }


        private IGateway getGateway()
        {
            return _manager.CreateInstance(gatewayType) as IGateway;
        }
    }
}