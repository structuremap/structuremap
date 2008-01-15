using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
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
        public void CallAllExpressionsOnConfigure()
        {
            MockRepository mocks = new MockRepository();
            IExpression expression1 = mocks.CreateMock<IExpression>();
            IExpression expression2 = mocks.CreateMock<IExpression>();
            IExpression expression3 = mocks.CreateMock<IExpression>();

            PluginGraph graph = new PluginGraph();
            expression1.Configure(graph);
            expression2.Configure(graph);
            expression3.Configure(graph);

            mocks.ReplayAll();

            TestRegistry registry = new TestRegistry(graph);
            registry.AddExpression(expression1);
            registry.AddExpression(expression2);
            registry.AddExpression(expression3);

            registry.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void DisposeCallsConfigure()
        {
            MockRepository mocks = new MockRepository();
            IExpression expression1 = mocks.CreateMock<IExpression>();
            IExpression expression2 = mocks.CreateMock<IExpression>();
            IExpression expression3 = mocks.CreateMock<IExpression>();

            PluginGraph graph = new PluginGraph();
            expression1.Configure(graph);
            expression2.Configure(graph);
            expression3.Configure(graph);

            mocks.ReplayAll();

            using (TestRegistry registry = new TestRegistry(graph))
            {
                registry.AddExpression(expression1);
                registry.AddExpression(expression2);
                registry.AddExpression(expression3);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void LoadControl()
        {
            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);

            string theUrl = "some url";
            string theKey = "the memento";
            registry.LoadControlFromUrl<IGateway>(theUrl).WithName(theKey);

            registry.Dispose();

            PluginFamily family = graph.PluginFamilies[typeof (IGateway)];
            UserControlMemento memento = (UserControlMemento) family.Source.GetMemento(theKey);
            Assert.IsNotNull(memento);

            Assert.AreEqual(theUrl, memento.Url);
            Assert.AreEqual(theKey, memento.InstanceKey);
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

        public void AddExpression(IExpression expression)
        {
            addExpression(expression);
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