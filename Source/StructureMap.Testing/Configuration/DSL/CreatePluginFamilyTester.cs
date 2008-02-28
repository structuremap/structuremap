using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
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

        [Test]
        public void AsAnotherScope()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression<IGateway> expression =
                    registry.BuildInstancesOf<IGateway>().CacheBy(InstanceScope.ThreadLocal);
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof (IGateway)];
            Assert.IsTrue(family.InterceptionChain.Contains(typeof (ThreadLocalStorageInterceptor)));
        }

        [Test]
        public void BuildInstancesOfType()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());
        }


        [Test]
        public void BuildPluginFamilyAsPerRequest()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression<IGateway> expression =
                    registry.BuildInstancesOf<IGateway>();
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof (IGateway)];
            Assert.AreEqual(0, family.InterceptionChain.Count);
        }

        [Test]
        public void BuildPluginFamilyAsSingleton()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression<IGateway> expression =
                    registry.BuildInstancesOf<IGateway>().AsSingletons();
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof (IGateway)];
            Assert.IsTrue(family.InterceptionChain.Contains(typeof (SingletonInterceptor)));
        }

        [Test]
        public void CanOverrideTheDefaultInstance1()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                // Specify the default implementation for an interface
                registry.BuildInstancesOf<IGateway>().TheDefaultIsConcreteType<StubbedGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway) manager.CreateInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (StubbedGateway), gateway);
        }

        [Test]
        public void CanOverrideTheDefaultInstanceAndCreateAnAllNewPluginOnTheFly()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>().TheDefaultIsConcreteType<FakeGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway) manager.CreateInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (FakeGateway), gateway);
        }

        [Test]
        public void CreatePluginFamilyWithADefault()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<IWidget>().TheDefaultIs(
                Registry.Instance<IWidget>().UsingConcreteType<ColorWidget>().WithProperty("Color").EqualTo("Red")
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            ColorWidget widget = (ColorWidget) manager.CreateInstance<IWidget>();
            Assert.AreEqual("Red", widget.Color);
        }

        [Test]
        public void PutAnInterceptorIntoTheInterceptionChainOfAPluginFamilyInTheDSL()
        {
            StubbedInstanceFactoryInterceptor factoryInterceptor = new StubbedInstanceFactoryInterceptor();

            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>().InterceptConstructionWith(factoryInterceptor);
            }

            InterceptionChain chain = pluginGraph.PluginFamilies[typeof (IGateway)].InterceptionChain;
            Assert.AreEqual(1, chain.Count);
            Assert.AreSame(factoryInterceptor, chain[0]);
        }

        [Test]
        public void TheDefaultInstanceIsConcreteType()
        {
            Registry registry = new Registry();

            // Needs to blow up if the concrete type can't be used
            registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<ARule>();

            IInstanceManager manager = registry.BuildInstanceManager();

            Assert.IsInstanceOfType(typeof (ARule), manager.CreateInstance<Rule>());
        }

        [Test]
        public void TheDefaultInstanceIsPickedUpFromTheAttribute()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway) manager.CreateInstance(typeof (IGateway));

            Assert.IsInstanceOfType(typeof (DefaultGateway), gateway);
        }

        [Test]
        public void AddInstanceByNameOnlyAddsOneInstanceToStructureMap()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<Something>().AddInstance(
                Registry.Instance<Something>().UsingConcreteType<RedSomething>().WithName("Red")
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            IList<Something> instances = manager.GetAllInstances<Something>();
            Assert.AreEqual(1, instances.Count);
        }

        [Test]
        public void AddInstanceWithNameOnlyAddsOneInstanceToStructureMap()
        {
            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);
            registry.AddInstanceOf<Something>().UsingConcreteType<RedSomething>().WithName("Red");

            

            IInstanceManager manager = registry.BuildInstanceManager();
            IList<Something> instances = manager.GetAllInstances<Something>();
            Assert.AreEqual(1, instances.Count);
        }
        
        public class Something{}

        public class RedSomething : Something{}
        public class GreenSomething : Something{}

    }

    public class StubbedInstanceFactoryInterceptor : InstanceFactoryInterceptor
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}