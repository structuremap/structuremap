using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System.Linq;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class CreatePluginFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public interface Something
        {
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

        [Test]
        public void Add_an_instance_by_lambda()
        {
            var container = new Container(r =>
            {
                r.For<IWidget>().Add(c => new AWidget());
            });

            container.GetAllInstances<IWidget>()
                .First()
                .ShouldBeOfType<AWidget>();
        }

        [Test]
        public void add_an_instance_by_literal_object()
        {
            var aWidget = new AWidget();

            var container = new Container(x =>
            {
                x.For<IWidget>().Use(aWidget);
            });

            container.GetAllInstances<IWidget>().First().ShouldBeTheSameAs(aWidget);
        }

        [Test]
        public void AddInstanceByNameOnlyAddsOneInstanceToStructureMap()
        {
            var container = new Container(r =>
            {
                r.For<Something>().Add<RedSomething>().Named("Red");
            });

            container.GetAllInstances<Something>().Count().ShouldEqual(1);
        }

        [Test]
        public void AddInstanceWithNameOnlyAddsOneInstanceToStructureMap()
        {
            var container = new Container(x => {
                x.For<Something>().Add<RedSomething>().Named("Red");
            });

            container.GetAllInstances<Something>()
                .Count().ShouldEqual(1);
        }

        [Test]
        public void AsAnotherScope()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.For<IGateway>(Lifecycles.ThreadLocal);
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();

            PluginFamily family = pluginGraph.Families[typeof(IGateway)];

            family.Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }

        [Test]
        public void BuildInstancesOfType()
        {
            var registry = new Registry();
            registry.For<IGateway>();
            PluginGraph pluginGraph = registry.Build();

            Assert.IsTrue(pluginGraph.Families.Has(typeof (IGateway)));
        }


        [Test]
        public void BuildPluginFamilyAsPerRequest()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.For<IGateway>();
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();

            PluginFamily family = pluginGraph.Families[typeof (IGateway)];
            family.Lifecycle.ShouldBeOfType<TransientLifecycle>();
        }

        [Test]
        public void BuildPluginFamilyAsSingleton()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.For<IGateway>().Singleton();
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();
            PluginFamily family = pluginGraph.Families[typeof (IGateway)];
            family.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void CanOverrideTheDefaultInstance1()
        {
            var registry = new Registry();
            // Specify the default implementation for an interface
            registry.For<IGateway>().Use<StubbedGateway>();

            PluginGraph pluginGraph = registry.Build();
            Assert.IsTrue(pluginGraph.Families.Has(typeof (IGateway)));

            var manager = new Container(pluginGraph);
            var gateway = (IGateway) manager.GetInstance(typeof (IGateway));

            gateway.ShouldBeOfType<StubbedGateway>();
        }

        [Test]
        public void CanOverrideTheDefaultInstanceAndCreateAnAllNewPluginOnTheFly()
        {
            var registry = new Registry();
            registry.For<IGateway>().Use<FakeGateway>();
            PluginGraph pluginGraph = registry.Build();

            Assert.IsTrue(pluginGraph.Families.Has(typeof (IGateway)));

            var container = new Container(pluginGraph);
            var gateway = (IGateway) container.GetInstance(typeof (IGateway));

            gateway.ShouldBeOfType<FakeGateway>();
        }

        [Test]
        public void CreatePluginFamilyWithADefault()
        {
            var container = new Container(r =>
            {
                r.For<IWidget>().Use<ColorWidget>()
                    .Ctor<string>("color").Is("Red");
            });

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }

        [Test]
        public void PutAnInterceptorIntoTheInterceptionChainOfAPluginFamilyInTheDSL()
        {
            var lifecycle = new StubbedLifecycle();

            var registry = new Registry();
            registry.For<IGateway>().LifecycleIs(lifecycle);

            PluginGraph pluginGraph = registry.Build();

            pluginGraph.Families[typeof (IGateway)]
                .Lifecycle.ShouldBeTheSameAs(lifecycle);
        }

        [Test]
        public void Set_the_default_by_a_lambda()
        {
            var manager =
                new Container(
                    registry => registry.For<IWidget>().Use(() => new AWidget()));

            manager.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
        }

        [Test]
        public void Set_the_default_to_a_built_object()
        {
            var aWidget = new AWidget();

            var manager =
                new Container(
                    registry => registry.For<IWidget>().Use(aWidget));

            Assert.AreSame(aWidget, manager.GetInstance<IWidget>());
        }

        [Test(
            Description =
                "Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en"
            )]
        public void TheDefaultInstanceIsALambdaForGuidNewGuid()
        {
            var manager =
                new Container(
                    registry => registry.For<Guid>().Use(() => Guid.NewGuid()));

            manager.GetInstance<Guid>().ShouldBeOfType<Guid>();
        }

        [Test]
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

        public string Scope { get { return "Stubbed"; } }
    }
}