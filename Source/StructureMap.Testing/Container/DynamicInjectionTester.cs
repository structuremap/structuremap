using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Interceptors;
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
            manager.AddInstance<IService>(new LiteralMemento(_red).Named("Red"));
            manager.AddInstance<IService>(new LiteralMemento(_blue).Named("Blue"));

            Assert.AreSame(_red, manager.CreateInstance(typeof (IService), "Red"));
            Assert.AreSame(_blue, manager.CreateInstance(typeof (IService), "Blue"));
        }

        [Test]
        public void AddInstanceWithInstanceFactoryInterceptor()
        {
            InstanceFactoryInterceptor interceptor = new FakeInstanceFactoryInterceptor();

            InstanceFactory factory = ObjectMother.Factory<IService>();
            interceptor.InnerInstanceFactory = factory;

            interceptor.AddInstance(new LiteralMemento(_red).Named("Red"));
            interceptor.AddInstance(new LiteralMemento(_blue).Named("Blue"));

            Assert.AreSame(_red, interceptor.GetInstance("Red"));
            Assert.AreSame(_blue, interceptor.GetInstance("Blue"));
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
            PluginFamily family = new PluginFamily(typeof (ISomething));
            family.Plugins.Add(typeof (SomethingOne), "One");

            InstanceFactory factory = new InstanceFactory(family, true);
            factory.SetInstanceManager(new InstanceManager());

            InstanceMemento memento = factory.AddType<SomethingOne>();
            Assert.AreEqual("One", memento.InstanceKey);
            Assert.AreEqual("One", memento.ConcreteKey);

            Assert.IsInstanceOfType(typeof (SomethingOne), factory.GetInstance(memento));

            IList list = factory.GetAllInstances();
            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
        }

        [Test]
        public void AddPluginForTypeWhenThePluginDoesNotAlreadyExistsDoesNothing()
        {
            InstanceFactory factory = ObjectMother.Factory<ISomething>();
            factory.SetInstanceManager(new InstanceManager());
            InstanceMemento memento = factory.AddType<SomethingOne>();

            Assert.IsNotNull(memento);

            Assert.IsInstanceOfType(typeof (SomethingOne), factory.GetInstance(memento));

            IList list = factory.GetAllInstances();
            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(typeof (SomethingOne), list[0]);
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
            InstanceFactory factory = ObjectMother.Factory<IService>();

            factory.AddInstance(new LiteralMemento(_red).Named("Red"));
            factory.AddInstance(new LiteralMemento(_blue).Named("Blue"));

            Assert.AreSame(_red, factory.GetInstance("Red"));
            Assert.AreSame(_blue, factory.GetInstance("Blue"));
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
            InstanceFactory factory = ObjectMother.Factory<IService>();

            factory.AddInstance(new LiteralMemento(_red).Named("Red"));
            factory.AddInstance(new LiteralMemento(_blue).Named("Blue"));

            // Replace Blue
            factory.AddInstance(new LiteralMemento(_orange).Named("Blue"));

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

    public class FakeInstanceFactoryInterceptor : InstanceFactoryInterceptor
    {
        public override object Clone()
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