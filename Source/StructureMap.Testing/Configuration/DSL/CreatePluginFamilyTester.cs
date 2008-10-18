using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

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
            public ClassWithStringInConstructor(string name){}
        }

        [Test]
        public void cannot_use_a_class_with_a_primitive_constructor_in_the_TheDefaultIsConcreteType_shortcut()
        {
            try
            {
                var container = new Container(x =>
                {
                    x.ForRequestedType<ClassWithStringInConstructor>().TheDefaultIsConcreteType
                        <ClassWithStringInConstructor>();
                });
                Assert.Fail("Should have thrown exception 231");
            }
            catch (StructureMapException e)
            {
                e.ErrorCode.ShouldEqual(231);
            }
        }

        [Test]
        public void Add_an_instance_by_lambda()
        {
            var manager =
                new Container(
                    r => r.InstanceOf<IWidget>().Is.ConstructedBy(() => new AWidget()));

            Assert.IsInstanceOfType(typeof (AWidget), manager.GetAllInstances<IWidget>()[0]);
        }

        [Test]
        public void Add_an_instance_by_literal()
        {
            var aWidget = new AWidget();

            var manager =
                new Container(registry => registry.InstanceOf<IWidget>().Is.Object(aWidget));

            Assert.IsInstanceOfType(typeof (AWidget), manager.GetAllInstances<IWidget>()[0]);
        }

        [Test]
        public void AddInstanceByNameOnlyAddsOneInstanceToStructureMap()
        {
            var container =
                new Container(r => { r.InstanceOf<Something>().Is.OfConcreteType<RedSomething>().WithName("Red"); });

            IList<Something> instances = container.GetAllInstances<Something>();
            Assert.AreEqual(1, instances.Count);
        }

        [Test]
        public void AddInstanceWithNameOnlyAddsOneInstanceToStructureMap()
        {
            IContainer manager =
                new Container(
                    registry => registry.InstanceOf<Something>().Is.OfConcreteType<RedSomething>().WithName("Red"));
            IList<Something> instances = manager.GetAllInstances<Something>();
            Assert.AreEqual(1, instances.Count);
        }

        [Test]
        public void AsAnotherScope()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.BuildInstancesOf<IGateway>().CacheBy(InstanceScope.ThreadLocal);
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();

            PluginFamily family = pluginGraph.FindFamily(typeof (IGateway));
            Assert.IsInstanceOfType(typeof (ThreadLocalStoragePolicy), family.Policy);
        }

        [Test]
        public void BuildInstancesOfType()
        {
            var registry = new Registry();
            registry.BuildInstancesOf<IGateway>();
            PluginGraph pluginGraph = registry.Build();

            Assert.IsTrue(pluginGraph.ContainsFamily(typeof (IGateway)));
        }


        [Test]
        public void BuildPluginFamilyAsPerRequest()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.BuildInstancesOf<IGateway>();
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();

            PluginFamily family = pluginGraph.FindFamily(typeof (IGateway));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family.Policy);
        }

        [Test]
        public void BuildPluginFamilyAsSingleton()
        {
            var registry = new Registry();
            CreatePluginFamilyExpression<IGateway> expression =
                registry.BuildInstancesOf<IGateway>().AsSingletons();
            Assert.IsNotNull(expression);

            PluginGraph pluginGraph = registry.Build();
            PluginFamily family = pluginGraph.FindFamily(typeof (IGateway));
            Assert.IsInstanceOfType(typeof (SingletonPolicy), family.Policy);
        }

        [Test]
        public void CanOverrideTheDefaultInstance1()
        {
            var registry = new Registry();
            // Specify the default implementation for an interface
            registry.BuildInstancesOf<IGateway>().TheDefaultIsConcreteType<StubbedGateway>();

            PluginGraph pluginGraph = registry.Build();
            Assert.IsTrue(pluginGraph.ContainsFamily(typeof (IGateway)));

            var manager = new Container(pluginGraph);
            var gateway = (IGateway) manager.GetInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (StubbedGateway), gateway);
        }

        [Test]
        public void CanOverrideTheDefaultInstanceAndCreateAnAllNewPluginOnTheFly()
        {
            var registry = new Registry();
            registry.BuildInstancesOf<IGateway>().TheDefaultIsConcreteType<FakeGateway>();
            PluginGraph pluginGraph = registry.Build();

            Assert.IsTrue(pluginGraph.ContainsFamily(typeof (IGateway)));

            var manager = new Container(pluginGraph);
            var gateway = (IGateway) manager.GetInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (FakeGateway), gateway);
        }

        [Test]
        public void CreatePluginFamilyWithADefault()
        {
            var container = new Container(r =>
            {
                r.ForRequestedType<IWidget>().TheDefault.Is.OfConcreteType<ColorWidget>()
                    .WithCtorArg("color").EqualTo("Red");
            });

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }

        [Test]
        public void PutAnInterceptorIntoTheInterceptionChainOfAPluginFamilyInTheDSL()
        {
            var factoryInterceptor = new StubbedInstanceFactoryInterceptor();

            var registry = new Registry();
            registry.BuildInstancesOf<IGateway>().InterceptConstructionWith(factoryInterceptor);

            PluginGraph pluginGraph = registry.Build();

            Assert.AreSame(pluginGraph.FindFamily(typeof (IGateway)).Policy, factoryInterceptor);
        }

        [Test]
        public void Set_the_default_by_a_lambda()
        {
            var manager =
                new Container(
                    registry => registry.ForRequestedType<IWidget>().TheDefault.Is.ConstructedBy(() => new AWidget()));

            Assert.IsInstanceOfType(typeof (AWidget), manager.GetInstance<IWidget>());
        }

        [Test]
        public void Set_the_default_to_a_built_object()
        {
            var aWidget = new AWidget();

            var manager =
                new Container(
                    registry => registry.ForRequestedType<IWidget>().TheDefault.Is.Object(aWidget));

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
                    registry => registry.ForRequestedType<Guid>().TheDefault.Is.ConstructedBy(() => Guid.NewGuid()));

            Assert.IsInstanceOfType(typeof (Guid), manager.GetInstance<Guid>());
        }

        [Test]
        public void TheDefaultInstanceIsConcreteType()
        {
            IContainer manager = new Container(
                registry => registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<ARule>());

            Assert.IsInstanceOfType(typeof (ARule), manager.GetInstance<Rule>());
        }

        [Test]
        public void TheDefaultInstanceIsPickedUpFromTheAttribute()
        {
            var registry = new Registry();
            registry.BuildInstancesOf<IGateway>();
            registry.Scan(x => x.AssemblyContainingType<IGateway>());

            PluginGraph pluginGraph = registry.Build();

            Assert.IsTrue(pluginGraph.ContainsFamily(typeof (IGateway)));

            var manager = new Container(pluginGraph);
            var gateway = (IGateway) manager.GetInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (DefaultGateway), gateway);
        }
    }

    public class StubbedInstanceFactoryInterceptor : IBuildInterceptor
    {
        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public IBuildPolicy Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}