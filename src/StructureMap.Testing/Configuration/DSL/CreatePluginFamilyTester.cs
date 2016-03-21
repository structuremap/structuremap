using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class CreatePluginFamilyTester
    {
        public interface SomethingElseEntirely : Something, SomethingElse
        {
        }

        public interface SomethingElse
        {
        }

        public interface Something
        {
        }

        public class OrangeSomething : SomethingElseEntirely
        {
            public readonly Guid Id = Guid.NewGuid();

            public override string ToString()
            {
                return string.Format("OrangeSomething: {0}", Id);
            }

            protected bool Equals(OrangeSomething other)
            {
                return Id.Equals(other.Id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((OrangeSomething)obj);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        public class RedSomething : Something
        {
        }

        public class GreenSomething : Something
        {
        }

        public class ClassWithStringInConstructor
        {
            public ClassWithStringInConstructor(string name)
            {
            }
        }

        [Fact]
        public void Add_an_instance_by_lambda()
        {
            var container = new Container(r => { r.For<IWidget>().Add(c => new AWidget()); });

            container.GetAllInstances<IWidget>()
                .First()
                .ShouldBeOfType<AWidget>();
        }

        [Fact]
        public void add_an_instance_by_literal_object()
        {
            var aWidget = new AWidget();

            var container = new Container(x => { x.For<IWidget>().Use(aWidget); });

            container.GetAllInstances<IWidget>().First().ShouldBeTheSameAs(aWidget);
        }

        [Fact]
        public void AddInstanceByNameOnlyAddsOneInstanceToStructureMap()
        {
            var container = new Container(r => { r.For<Something>().Add<RedSomething>().Named("Red"); });

            container.GetAllInstances<Something>().Count().ShouldBe(1);
        }

        [Fact]
        public void AddInstanceWithNameOnlyAddsOneInstanceToStructureMap()
        {
            var container = new Container(x => { x.For<Something>().Add<RedSomething>().Named("Red"); });

            container.GetAllInstances<Something>()
                .Count().ShouldBe(1);
        }

        [Fact]
        public void as_another_lifecycle()
        {
            var registry = new Registry();
            registry.For<IGateway>(Lifecycles.ThreadLocal).ShouldNotBeNull();

            var pluginGraph = registry.Build();

            var family = pluginGraph.Families[typeof(IGateway)];

            family.Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }

        [Fact]
        public void BuildInstancesOfType()
        {
            var registry = new Registry();
            registry.For<IGateway>();
            var pluginGraph = registry.Build();

            pluginGraph.Families.Has(typeof(IGateway)).ShouldBeTrue();
        }

        [Fact]
        public void BuildPluginFamilyAsPerRequest()
        {
            var registry = new Registry();

            var pluginGraph = registry.Build();

            var family = pluginGraph.Families[typeof(IGateway)];
            family.Lifecycle.ShouldBeNull();
        }

        [Fact]
        public void BuildPluginFamilyAsSingleton()
        {
            var registry = new Registry();
            registry.For<IGateway>().Singleton()
                .ShouldNotBeNull();

            var pluginGraph = registry.Build();
            var family = pluginGraph.Families[typeof(IGateway)];
            family.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Fact]
        public void CanOverrideTheDefaultInstance1()
        {
            var registry = new Registry();
            // Specify the default implementation for an interface
            registry.For<IGateway>().Use<StubbedGateway>();

            var pluginGraph = registry.Build();
            pluginGraph.Families.Has(typeof(IGateway)).ShouldBeTrue();

            var manager = new Container(pluginGraph);
            var gateway = (IGateway)manager.GetInstance(typeof(IGateway));

            gateway.ShouldBeOfType<StubbedGateway>();
        }

        [Fact]
        public void CanOverrideTheDefaultInstanceAndCreateAnAllNewPluginOnTheFly()
        {
            var registry = new Registry();
            registry.For<IGateway>().Use<FakeGateway>();
            var pluginGraph = registry.Build();

            pluginGraph.Families.Has(typeof(IGateway)).ShouldBeTrue();

            var container = new Container(pluginGraph);
            var gateway = (IGateway)container.GetInstance(typeof(IGateway));

            gateway.ShouldBeOfType<FakeGateway>();
        }

        [Fact]
        public void CreatePluginFamilyWithADefault()
        {
            var container = new Container(r =>
            {
                r.For<IWidget>().Use<ColorWidget>()
                    .Ctor<string>("color").Is("Red");
            });

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldBe("Red");
        }

        [Fact]
        public void weird_generics_casting()
        {
            typeof(SomethingElseEntirely).CanBeCastTo<SomethingElse>()
                .ShouldBeTrue();
        }

        [Fact]
        public void CreatePluginFamilyWithReferenceToAnotherFamily()
        {
            var container = new Container(r =>
            {
                // Had to be a SingletonThing for this to work
                r.ForSingletonOf<SomethingElseEntirely>().Use<OrangeSomething>();
                r.For<SomethingElse>().Use(context =>
                    // If the return is cast to OrangeSomething, this works.
                    context.GetInstance<SomethingElseEntirely>());
                r.For<Something>().Use(context =>
                    // If the return is cast to OrangeSomething, this works.
                    context.GetInstance<SomethingElseEntirely>());
            });

            var orangeSomething = container.GetInstance<SomethingElseEntirely>();
            orangeSomething.ShouldBeOfType<OrangeSomething>();

            container.GetInstance<SomethingElse>()
                .ShouldBeOfType<OrangeSomething>()
                .ShouldBe(orangeSomething);

            container.GetInstance<Something>()
                .ShouldBeOfType<OrangeSomething>()
                .ShouldBe(orangeSomething);
        }

        [Fact]
        public void PutAnInterceptorIntoTheInterceptionChainOfAPluginFamilyInTheDSL()
        {
            var lifecycle = new StubbedLifecycle();

            var registry = new Registry();
            registry.For<IGateway>().LifecycleIs(lifecycle);

            var pluginGraph = registry.Build();

            pluginGraph.Families[typeof(IGateway)].Lifecycle.ShouldBeTheSameAs(lifecycle);
        }

        [Fact]
        public void Set_the_default_by_a_lambda()
        {
            var manager =
                new Container(
                    registry => registry.For<IWidget>().Use(() => new AWidget()));

            manager.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
        }

        [Fact]
        public void Set_the_default_to_a_built_object()
        {
            var aWidget = new AWidget();

            var manager =
                new Container(
                    registry => registry.For<IWidget>().Use(aWidget));

            aWidget.ShouldBeTheSameAs(manager.GetInstance<IWidget>());
        }

        // Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en
        [Fact]
        public void TheDefaultInstanceIsALambdaForGuidNewGuid()
        {
            var manager =
                new Container(
                    registry => registry.For<Guid>().Use(() => Guid.NewGuid()));

            manager.GetInstance<Guid>().ShouldBeOfType<Guid>();
        }

        [Fact]
        public void TheDefaultInstanceIsConcreteType()
        {
            IContainer manager = new Container(
                registry => registry.For<Rule>().Use<ARule>());

            manager.GetInstance<Rule>().ShouldBeOfType<ARule>();
        }
    }

    public class StubbedLifecycle : ILifecycle
    {
        public void EjectAll(ILifecycleContext context)
        {
            throw new NotImplementedException();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            throw new NotImplementedException();
        }

        public string Description
        {
            get { return "Stubbed"; }
        }
    }
}