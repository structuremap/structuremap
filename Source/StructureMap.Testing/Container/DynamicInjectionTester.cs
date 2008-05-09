using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
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

        private IService _red = new ColorService("Red");
        private IService _blue = new ColorService("Blue");
        private IService _orange = new ColorService("Orange");

        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists()
        {
            StructureMapConfiguration.BuildInstancesOf<ISomething>().TheDefaultIsConcreteType<SomethingTwo>();

            ObjectFactory.InjectDefaultType<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }


        [Test]
        public void AddANewDefaultTypeForAPluginTypeThatAlreadyExists2()
        {
            StructureMapConfiguration.BuildInstancesOf<ISomething>();

            ObjectFactory.InjectDefaultType<ISomething, SomethingOne>();
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
            InstanceManager manager = new InstanceManager();
            manager.AddInstance<IService>(new LiteralInstance(_red).WithName("Red"));
            manager.AddInstance<IService>(new LiteralInstance(_blue).WithName("Blue"));

            Assert.AreSame(_red, manager.CreateInstance(typeof (IService), "Red"));
            Assert.AreSame(_blue, manager.CreateInstance(typeof (IService), "Blue"));
        }


        [Test]
        public void AddNamedInstanceByType()
        {
            ObjectFactory.InjectByName<ISomething, SomethingOne>("One");
            ObjectFactory.InjectByName<ISomething, SomethingTwo>("Two");

            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetNamedInstance<ISomething>("One"));
            Assert.IsInstanceOfType(typeof (SomethingTwo), ObjectFactory.GetNamedInstance<ISomething>("Two"));
        }

        [Test]
        public void AddNamedInstanceToobjectFactory()
        {
            SomethingOne one = new SomethingOne();
            SomethingOne two = new SomethingOne();

            ObjectFactory.InjectByName<ISomething>(one, "One");
            ObjectFactory.InjectByName<ISomething>(two, "Two");


            Assert.AreSame(one, ObjectFactory.GetNamedInstance<ISomething>("One"));
            Assert.AreSame(two, ObjectFactory.GetNamedInstance<ISomething>("Two"));
        }

        [Test]
        public void AddPluginForTypeWhenThePluginAlreadyExists()
        {
            PluginGraph pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.FindFamily(typeof(ISomething));
            family.Plugins.Add(typeof (SomethingOne), "One");

            InstanceManager manager = new InstanceManager(pluginGraph);
            IInstanceFactory factory = manager[typeof (ISomething)];

            ConfiguredInstance instance = (ConfiguredInstance) factory.AddType<SomethingOne>();
            Assert.AreEqual("One", instance.Name);
            Assert.AreEqual("One", instance.ConcreteKey);

            Assert.IsInstanceOfType(typeof(SomethingOne), factory.GetInstance(instance, manager));

            IList list = factory.GetAllInstances();
            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof(SomethingOne), list[0]);
        }

        [Test]
        public void AddPluginForTypeWhenThePluginDoesNotAlreadyExistsDoesNothing()
        {
            PluginGraph pluginGraph = new PluginGraph();
            InstanceManager manager = new InstanceManager(pluginGraph);

            manager.AddInstance<ISomething, SomethingOne>();

            IList<ISomething> list = manager.GetAllInstances<ISomething>();

            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof(SomethingOne), list[0]);
        }

        [Test]
        public void AddTypeThroughObjectFactory()
        {
            ObjectFactory.InjectDefaultType<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void CanAddMementosDirectlyToAnInstanceFactory()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new LiteralInstance(_red).WithName("Red"));
            factory.AddInstance(new LiteralInstance(_blue).WithName("Blue"));

            Assert.AreSame(_red, factory.GetInstance("Red"));
            Assert.AreSame(_blue, factory.GetInstance("Blue"));
        }

        private IInstanceFactory getISomethingFactory()
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.FindFamily(typeof (ISomething));
            InstanceManager manager = new InstanceManager(pluginGraph);
            return manager[typeof(ISomething)];
        }

        [Test]
        public void InjectType()
        {
            ObjectFactory.AddType<ISomething, SomethingOne>();
            IList<ISomething> list = ObjectFactory.GetAllInstances<ISomething>();

            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
        }

        [Test]
        public void JustAddATypeWithNoNameAndDefault()
        {
            ObjectFactory.InjectDefaultType<ISomething, SomethingOne>();
            Assert.IsInstanceOfType(typeof (SomethingOne), ObjectFactory.GetInstance<ISomething>());
        }

        [Test]
        public void NowOverwriteAPreviouslyAttachedMemento()
        {
            IInstanceFactory factory = getISomethingFactory();

            factory.AddInstance(new LiteralInstance(_red).WithName("Red"));
            factory.AddInstance(new LiteralInstance(_blue).WithName("Blue"));

            // Replace Blue
            factory.AddInstance(new LiteralInstance(_orange).WithName("Blue"));

            Assert.AreSame(_orange, factory.GetInstance("Blue"));
        }

        [Test]
        public void OverwriteInstanceAndDontBlowUp()
        {
            ObjectFactory.InjectByName<ISomething, SomethingOne>("One");
            ObjectFactory.InjectByName<ISomething, SomethingTwo>("One");

            Assert.IsInstanceOfType(typeof (SomethingTwo), ObjectFactory.GetNamedInstance<ISomething>("One"));
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
    }

    public class FakeInstanceFactoryInterceptor : IBuildInterceptor
    {
        public IBuildPolicy InnerPolicy
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object Build(IInstanceCreator instanceCreator, Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        IBuildPolicy IBuildPolicy.Clone()
        {
            throw new NotImplementedException();
        }
    }

    public interface ISomething
    {
        void Go();
    }

    public class SomethingOne : ISomething
    {
        #region ISomething Members

        public void Go()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SomethingTwo : ISomething
    {
        #region ISomething Members

        public void Go()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}