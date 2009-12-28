using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
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

        public class RedGreenRegistry : Registry
        {
            public RedGreenRegistry()
            {
                InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Red").WithName("Red");
                InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Green").WithName(
                    "Green");
            }
        }

        public class YellowBlueRegistry : Registry
        {
            public YellowBlueRegistry()
            {
                InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Yellow").WithName(
                    "Yellow");
                InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithProperty("color").EqualTo("Blue").WithName("Blue");
            }
        }

        public class PurpleRegistry : Registry
        {
            public PurpleRegistry()
            {
                For<IWidget>().Use<AWidget>();
            }
        }

        [Test]
        public void include_a_registry()
        {
            var registry = new Registry();
            registry.IncludeRegistry<YellowBlueRegistry>();
            registry.IncludeRegistry<RedGreenRegistry>();
            registry.IncludeRegistry<PurpleRegistry>();

            var container = new Container(registry);

            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();

            container.GetAllInstances<IWidget>().Count.ShouldEqual(5);
        }


        [Test]
        public void Can_add_an_instance_for_concrete_class_with_no_constructors()
        {
            var registry = new Registry();
            registry.ForRequestedType<ConcreteWithNoConstructor>().TheDefault.Is.ConstructedBy(
                () => ConcreteWithNoConstructor.Build());

            var container = new Container(registry);
        }

        [Test]
        public void Equals_check_true()
        {
            var registry1 = new TestRegistry();
            var registry2 = new TestRegistry();
            var registry3 = new TestRegistry2();
            var registry4 = new TestRegistry2();

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
            var registry2 = new TestRegistry2();
            var graph = new PluginGraph();

            graph.Registries.Count.ShouldEqual(0);
            registry2.ConfigurePluginGraph(graph);

            graph.Registries.Contains(registry2).ShouldBeTrue();

            registry2.ConfigurePluginGraph(graph);
            registry2.ExecutedCount.ShouldEqual(1);
        }

        [Test]
        public void use_the_basic_actions_as_part_of_building_a_PluginGraph()
        {
            var container = new Container(new BasicActionRegistry());
            container.GetInstance<IGateway>().ShouldBeOfType<Fake3Gateway>();
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
        private readonly int _count;

        public TestRegistry2()
        {
            _count++;
        }

        public int ExecutedCount { get { return _count; } }
    }

    public class FakeGateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI { get { throw new NotImplementedException(); } }

        #endregion
    }

    public class Fake2Gateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI { get { throw new NotImplementedException(); } }

        #endregion
    }

    public class Fake3Gateway : IGateway
    {
        #region IGateway Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        public string WhoAmI { get { throw new NotImplementedException(); } }

        #endregion
    }

    public class BasicActionRegistry : Registry
    {
        public BasicActionRegistry()
        {
            registerAction(() => ForRequestedType<IGateway>().TheDefaultIsConcreteType<Fake3Gateway>());
        }
    }
}