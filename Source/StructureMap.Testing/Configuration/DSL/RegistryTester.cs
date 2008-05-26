using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class RegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion


        [Test]
        public void LoadControl()
        {
            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);

            string theUrl = "some url";
            string theKey = "the memento";
            registry.LoadControlFromUrl<IGateway>(theUrl).WithName(theKey);

            registry.Dispose();

            PluginFamily family = graph.FindFamily(typeof (IGateway));
            UserControlInstance instance = (UserControlInstance) family.GetInstance(theKey);
            Assert.IsNotNull(instance);

            Assert.AreEqual(theUrl, instance.Url);
            Assert.AreEqual(theKey, instance.Name);
        }
    }

    public class TestRegistry : Registry
    {
        public TestRegistry(PluginGraph graph) : base(graph)
        {
        }


        public TestRegistry()
        {
        }

    }

    public class FakeGateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class Fake2Gateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class Fake3Gateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}