using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Container.Interceptors
{
    [TestFixture]
    public class TypeInterceptionTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            manager = null;

            registry = new Registry();
            registry.ForRequestedType<IAnInterfaceOfSomeSort>()
                .AddInstance(Instance<RedSomething>().WithName("Red"))
                .AddInstance(Instance<GreenSomething>().WithName("Green"))
                .AddInstance(Instance<BlueSomething>().WithName("Blue"));
        }

        #endregion

        private Registry registry;
        private IInstanceManager manager;

        private void assertThisIsType<T>(string name)
        {
            if (manager == null)
            {
                manager = new InstanceManager(registry);
            }

            Assert.IsInstanceOfType(typeof (T), manager.CreateInstance<IAnInterfaceOfSomeSort>(name));
        }

        private void assertThatThisIsWrappedSomething<OUTERTYPE, INNERTYPE>(string name)
            where OUTERTYPE : WrappedSomething
        {
            if (manager == null)
            {
                manager = new InstanceManager(registry);
            }

            OUTERTYPE something = (OUTERTYPE) manager.CreateInstance<IAnInterfaceOfSomeSort>(name);
            Assert.IsInstanceOfType(typeof (INNERTYPE), something.Inner);
        }

        public interface IAnInterfaceOfSomeSort
        {
        }

        public class RedSomething : IAnInterfaceOfSomeSort
        {
        }

        public class GreenSomething : IAnInterfaceOfSomeSort
        {
        }

        public class BlueSomething : IAnInterfaceOfSomeSort
        {
        }

        public class WrappedSomething : IAnInterfaceOfSomeSort
        {
            private readonly IAnInterfaceOfSomeSort _inner;

            public WrappedSomething(IAnInterfaceOfSomeSort inner)
            {
                _inner = inner;
            }


            public IAnInterfaceOfSomeSort Inner
            {
                get { return _inner; }
            }
        }

        public class WrappedSomething2 : WrappedSomething
        {
            public WrappedSomething2(IAnInterfaceOfSomeSort inner) : base(inner)
            {
            }
        }

        [Test]
        public void If_An_Interceptor_Is_Registered_At_The_PluginGraph_It_Will_Be_Used_To_Construct_An_Instance()
        {
            MockTypeInterceptor interceptor = new MockTypeInterceptor();
            interceptor.AddHandler<RedSomething>(
                delegate(RedSomething something) { return new WrappedSomething(something); });

            interceptor.AddHandler<GreenSomething>(
                delegate(GreenSomething something) { return new WrappedSomething2(something); });

            registry.RegisterInterceptor(interceptor);

            assertThisIsType<BlueSomething>("Blue");
            assertThatThisIsWrappedSomething<WrappedSomething, RedSomething>("Red");
            assertThatThisIsWrappedSomething<WrappedSomething2, GreenSomething>("Green");
        }

        [Test]
        public void Register_A_Type_Interceptor_By_The_Fluent_Interface()
        {
            registry.IfTypeMatches(delegate(Type type) { return type.Equals(typeof (BlueSomething)); })
                .InterceptWith(
                delegate(object rawInstance) { return new WrappedSomething((IAnInterfaceOfSomeSort) rawInstance); });

            assertThisIsType<RedSomething>("Red");
            assertThisIsType<GreenSomething>("Green");
            assertThatThisIsWrappedSomething<WrappedSomething, BlueSomething>("Blue");
        }
    }
}