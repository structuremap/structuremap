using Shouldly;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class RegistryTester
    {
        public class RedGreenRegistry : Registry
        {
            public RedGreenRegistry()
            {
                For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Red").Named(
                    "Red");
                For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Green").Named(
                    "Green");
            }
        }

        public class YellowBlueRegistry : Registry
        {
            public YellowBlueRegistry()
            {
                For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Yellow").Named(
                    "Yellow");
                For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Blue").Named(
                    "Blue");
            }
        }

        // SAMPLE: simple-registry
        public class PurpleRegistry : Registry
        {
            public PurpleRegistry()
            {
                For<IWidget>().Use<AWidget>();
            }
        }

        // ENDSAMPLE

        [Fact]
        public void Can_add_an_instance_for_concrete_class_with_no_constructors()
        {
            var registry = new Registry();
            registry.For<ConcreteWithNoConstructor>().Use(
                c => ConcreteWithNoConstructor.Build());

            var container = new Container(registry);
            container.GetInstance<ConcreteWithNoConstructor>().ShouldNotBeNull();
        }

        [Fact]
        public void an_instance_of_the_base_registry_is_equal_to_itself()
        {
            var registry1 = new Registry();

            registry1.Equals((object)registry1).ShouldBeTrue();
        }

        [Fact]
        public void two_instances_of_the_base_registry_type_are_not_considered_equal()
        {
            var registry1 = new Registry();
            var registry2 = new Registry();

            registry1.Equals((object)registry2).ShouldBeFalse();
        }

        [Fact]
        public void two_instances_of_a_public_derived_registry_type_are_considered_equal()
        {
            var registry1 = new TestRegistry();
            var registry2 = new TestRegistry();
            var registry3 = new TestRegistry2();
            var registry4 = new TestRegistry2();

            registry1.Equals((object)registry1).ShouldBeTrue();
            registry1.Equals((object)registry2).ShouldBeTrue();
            registry2.Equals((object)registry1).ShouldBeTrue();
            registry3.Equals((object)registry4).ShouldBeTrue();

            registry1.Equals((object)registry3).ShouldBeFalse();
            registry3.Equals((object)registry1).ShouldBeFalse();
        }

        [Fact]
        public void two_instances_of_a_non_public_derived_registry_type_are_not_considered_equal()
        {
            var registry1 = new InternalTestRegistry();
            var registry2 = new InternalTestRegistry();

            registry1.Equals((object)registry1).ShouldBeTrue();
            registry1.Equals((object)registry2).ShouldBeFalse();
        }

        // SAMPLE: including-registries
        [Fact]
        public void include_a_registry()
        {
            var registry = new Registry();
            registry.IncludeRegistry<YellowBlueRegistry>();
            registry.IncludeRegistry<RedGreenRegistry>();
            registry.IncludeRegistry<PurpleRegistry>();
            // build a container
            var container = new Container(registry);
            // verify the default implementation and total registered implementations
            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetAllInstances<IWidget>().Count().ShouldBe(5);
        }

        // ENDSAMPLE

        public class MutatedWidget : IWidget
        {
            public void DoSomething()
            {
            }
        }

        public class MutatingRegistry : Registry
        {
            private static int count;

            public MutatingRegistry()
            {
                For<IWidget>().Use<AWidget>();

                if (count++ >= 1)
                {
                    For<IWidget>().Use<MutatedWidget>();
                }
            }
        }

        [Fact]
        public void Latch_on_a_PluginGraph()
        {
            var registry2 = new TestRegistry2();
            var graph = PluginGraph.CreateRoot();

            graph.ImportRegistry(registry2);

            graph.QueuedRegistries.Count.ShouldBe(1);

            graph.ImportRegistry(registry2);
            graph.QueuedRegistries.Count.ShouldBe(1);
        }

        [Fact]
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

        public int ExecutedCount
        {
            get { return _count; }
        }
    }

    internal class InternalTestRegistry : Registry
    {
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

        #endregion IGateway Members
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

        #endregion IGateway Members
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

        #endregion IGateway Members
    }

    public class BasicActionRegistry : Registry
    {
        public BasicActionRegistry()
        {
            For<IGateway>().Use<Fake3Gateway>();
        }
    }
}