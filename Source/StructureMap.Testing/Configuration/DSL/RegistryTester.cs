using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
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
        public void Equals_check_true()
        {
            TestRegistry registry1 = new TestRegistry();
            TestRegistry registry2 = new TestRegistry();
            TestRegistry2 registry3 = new TestRegistry2();
            TestRegistry2 registry4 = new TestRegistry2();

            registry1.Equals(registry1).ShouldBeTrue();
            registry1.Equals(registry2).ShouldBeTrue();
            registry2.Equals(registry1).ShouldBeTrue();
            registry3.Equals(registry4).ShouldBeTrue();

            registry1.Equals(registry3).ShouldBeFalse();
            registry3.Equals(registry1).ShouldBeFalse();
        }

        [Test]
        public void Latch_on_a_PluginGraph()
        {
            TestRegistry2 registry2 = new TestRegistry2();
            PluginGraph graph = new PluginGraph();

            graph.Registries.Count.ShouldEqual(0);
            registry2.ConfigurePluginGraph(graph);

            graph.Registries.Contains(registry2).ShouldBeTrue();
        
            registry2.ConfigurePluginGraph(graph);
            registry2.ExecutedCount.ShouldEqual(1);
        }

        [Test]
        public void Can_add_an_instance_for_concrete_class_with_no_constructors()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<ConcreteWithNoConstructor>().TheDefaultIs(() => ConcreteWithNoConstructor.Build());

            Container container = new Container(registry);

        }
    }

    public class ConcreteWithNoConstructor
    {
        private ConcreteWithNoConstructor()
        {
        }

        public static ConcreteWithNoConstructor Build()
        {
            return new ConcreteWithNoConstructor();
        }
    }

    public class TestRegistry : Registry
    {
    }

    public class TestRegistry2 : Registry
    {
        private int _count;

        public int ExecutedCount
        {
            get { return _count; }
        }

        protected override void configure()
        {
            _count++;
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