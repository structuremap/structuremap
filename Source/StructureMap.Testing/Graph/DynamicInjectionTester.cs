using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class DynamicInjectionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ObjectFactory.ReInitialize();
            StructureMapConfiguration.ResetAll();
        }

        #endregion

        private readonly IService _red = new ColorService("Red");
        private readonly IService _blue = new ColorService("Blue");
        private readonly IService _orange = new ColorService("Orange");

        private IInstanceFactory getISomethingFactory()
        {
            PluginFamily family = new PluginFamily(typeof (ISomething));
            return new InstanceFactory(family);
        }

        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists()
        {
            StructureMapConfiguration.BuildInstancesOf<ISomething>().TheDefaultIsConcreteType<SomethingTwo>();
            ObjectFactory.SetDefault<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }


        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists2()
        {
            StructureMapConfiguration.BuildInstancesOf<ISomething>();

            ObjectFactory.SetDefault<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void AddInstanceFromObjectFactory()
        {
            SomethingOne one = new SomethingOne();
            ObjectFactory.Inject<ISomething>(one);

            Assert.AreSame(one, ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void AddInstanceToInstanceManagerWhenTheInstanceFactoryDoesNotExist()
        {
            IContainer container = new Container(new PluginGraph());
            container.Configure(delegate(Registry registry)
            {
                registry.AddInstanceOf(_red).WithName("Red");
                registry.AddInstanceOf(_blue).WithName("Blue");
            });

            Assert.AreSame(_red, container.GetInstance(typeof (IService), "Red"));
            Assert.AreSame(_blue, container.GetInstance(typeof (IService), "Blue"));
        }


        [Test]
        public void AddNamedInstanceByType()
        {
            ObjectFactory.Configure(delegate(Registry registry)
            {
                registry.AddInstanceOf<ISomething>().UsingConcreteType<SomethingOne>().WithName("One");
                registry.AddInstanceOf<ISomething>().UsingConcreteType<SomethingTwo>().WithName("Two");
            });

            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetNamedInstance<ISomething>("One"));
            Assert.IsInstanceOfType(typeof (SomethingTwo), ObjectFactory.GetNamedInstance<ISomething>("Two"));
        }

        [Test]
        public void AddNamedInstanceToobjectFactory()
        {
            SomethingOne one = new SomethingOne();
            SomethingOne two = new SomethingOne();

            ObjectFactory.Configure(delegate(Registry registry)
            {
                registry.AddInstanceOf<ISomething>(one).WithName("One");
                registry.AddInstanceOf<ISomething>(two).WithName("Two");
            });

            Assert.AreSame(one, ObjectFactory.GetNamedInstance<ISomething>("One"));
            Assert.AreSame(two, ObjectFactory.GetNamedInstance<ISomething>("Two"));
        }

        [Test]
        public void AddPluginForTypeWhenThePluginAlreadyExists()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof (ISomething));
            family.AddPlugin(typeof (SomethingOne), "One");

            IContainer manager = new Container(pluginGraph);

            manager.Configure(
                delegate(Registry registry)
                {
                    registry.ForRequestedType<ISomething>().AliasConcreteType<SomethingOne>("One");
                    registry.AddInstanceOf<ISomething>().WithConcreteKey("One").WithName("One");
                });

            IList<ISomething> list = manager.GetAllInstances<ISomething>();
            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
        }

        [Test]
        public void AddPluginForTypeWhenThePluginDoesNotAlreadyExistsDoesNothing()
        {
            PluginGraph pluginGraph = new PluginGraph();
            IContainer container = new Container(pluginGraph);
            container.Configure(
                delegate(Registry registry) { registry.AddInstanceOf<ISomething>().UsingConcreteType<SomethingOne>(); });

            IList<ISomething> list = container.GetAllInstances<ISomething>();

            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
        }

        [Test]
        public void AddTypeThroughObjectFactory()
        {
            ObjectFactory.SetDefault<ISomething, SomethingOne>();

            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void CanAddInstancesDirectlyToAnInstanceFactory()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new LiteralInstance(_red).WithName("Red"));
            factory.AddInstance(new LiteralInstance(_blue).WithName("Blue"));


            Assert.AreSame(_red, factory.Build(new StubBuildSession(), factory.FindInstance("Red")));
            Assert.AreSame(_blue, factory.Build(new StubBuildSession(), factory.FindInstance("Blue")));
        }

        [Test]
        public void InjectType()
        {
            ObjectFactory.Configure(
                delegate(Registry registry) { registry.AddInstanceOf<ISomething>().UsingConcreteType<SomethingOne>(); });

            IList<ISomething> list = ObjectFactory.GetAllInstances<ISomething>();

            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
        }

        [Test]
        public void JustAddATypeWithNoNameAndDefault()
        {
            ObjectFactory.SetDefault<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void NowOverwriteAPreviouslyAttachedInstance()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new LiteralInstance(_red).WithName("Red"));
            factory.AddInstance(new LiteralInstance(_blue).WithName("Blue"));

            // Replace Blue
            factory.AddInstance(new LiteralInstance(_orange).WithName("Blue"));

            Assert.AreSame(_orange, factory.Build(new StubBuildSession(), factory.FindInstance("Blue")));
        }

        [Test]
        public void OverwriteInstanceFromObjectFactory()
        {
            SomethingOne one = new SomethingOne();
            SomethingOne two = new SomethingOne();
            ObjectFactory.Inject<ISomething>(one);
            ObjectFactory.Inject<ISomething>(two);

            Assert.AreSame(two, ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void Add_generic_stuff_in_configure()
        {
            Container container = new Container();
            container.Configure(delegate(Registry registry)
            {
                registry.ForRequestedType(typeof (IService<>))
                    .AddConcreteType(typeof (Service1<>))
                    .AddConcreteType(typeof (Service2<>));
            });

            Assert.AreEqual(2, container.GetAllInstances<IService<string>>().Count);
        }

        [Test]
        public void Add_an_assembly_in_the_Configure()
        {
            Container container = new Container();
            container.Configure(delegate(Registry registry)
            {
                registry.ScanAssemblies().IncludeTheCallingAssembly();
            });

            Assert.IsInstanceOfType(typeof(TheThingy), container.GetInstance<IThingy>());
        }

        [Test]
        public void Add_an_assembly_on_the_fly_and_pick_up_plugins()
        {
            Container container = new Container();
            container.Configure(delegate(Registry registry)
            {
                registry.ScanAssemblies().IncludeTheCallingAssembly().AddAllTypesOf<IWidget>();
            });

            IList<IWidget> instances = container.GetAllInstances<IWidget>();
            bool found = false;
            foreach (IWidget widget in instances)
            {
                found |= widget.GetType().Equals(typeof (TheWidget));
            }

            Assert.IsTrue(found);
        }

        public interface IService<T>{}
        public class Service1<T> : IService<T> { }
        public class Service2<T> : IService<T> { }
        public class Service3<T> : IService<T> { }

        [PluginFamily("Default")]
        public interface IThingy{}

        [Pluggable("Default")]
        public class TheThingy : IThingy{}


        public class TheWidget : IWidget
        {
            public void DoSomething()
            {
                throw new NotImplementedException();
            }
        }
    }

    public class FakeInstanceFactoryInterceptor : IBuildInterceptor
    {
        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object Build(IBuildSession buildSession, Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        IBuildPolicy IBuildPolicy.Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SomethingOne : ISomething
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }

    public class SomethingTwo : ISomething
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }
}