using Castle.DynamicProxy;
using Moq;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoFactory;

namespace StructureMap.Testing.AutoFactories
{
    [TestFixture]
    public class ProxyFactoryTests
    {
        public interface IFactory
        {
            IService CreateService();
        }

        public interface IService
        {
            
        }

        public class Service : IService { }

        private static TDependency Stub<TDependency>() where TDependency : class
        {
            return MockRepository.GenerateStub<TDependency>();
        }

        [Test]
        public void Should_create_a_proxy_factory()
        {
            var proxyGenerator = new ProxyGenerator();
            var context = Stub<IContext>();

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context);

            var factory = proxyFactory.Create();

            factory.ShouldNotBeNull();
        }

        [Test]
        public void Should_build_the_service_by_return_type_from_context()
        {
            var proxyGenerator = new ProxyGenerator();
            var context = Stub<IContext>();

            var service = new Service();

            context.Stub(x => x.GetInstance(typeof(IService))).Return(service);

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context);

            var factory = proxyFactory.Create();

            var built = factory.CreateService();

            built.ShouldNotBeNull();
            built.ShouldBeTheSameAs(service);
        }

        [TestFixture]
        public class WhenUsingExtraParameters
        {
            #region Types used for testing
            public class SingleParameter
            {
                public string Name;
                public SingleParameter(string name)
                {
                    Name = name;
                }
            }

            public class SeveralPrimitives
            {
                public readonly int TheAnswer;
                public readonly bool IsTrue;

                public SeveralPrimitives(int theAnswer, bool isTrue)
                {
                    TheAnswer = theAnswer;
                    IsTrue = isTrue;
                }
            }

            public class SeveralPrimitivesAndService
            {
                public readonly int TheAnswer;
                public readonly bool IsTrue;
                private readonly IService service;

                public SeveralPrimitivesAndService(int theAnswer, bool isTrue, IService service)
                {
                    TheAnswer = theAnswer;
                    IsTrue = isTrue;
                    this.service = service;
                }
            }

            public interface IFactory
            {
                SingleParameter GetSingleParameter(string name);
                SeveralPrimitives GetSeveralPrimitives(int theAnswer, bool isTrue);
                SeveralPrimitivesAndService GetSeveralPrimitivesAndService(int theAnswer, bool isTrue);
            }
            #endregion

            [Test]
            public void It_uses_the_name_of_the_parameter_as_a_key()
            {
                using (var container = new Container(cfg => cfg.For<IFactory>().CreateFactory()))
                {
                    var factory = container.GetInstance<IFactory>();
                    var value = factory.GetSingleParameter("foo");
                    Assert.AreEqual("foo", value.Name);
                }
            }

            [Test]
            public void It_can_do_several_parameters()
            {
                using (var container = new Container(cfg => cfg.For<IFactory>().CreateFactory()))
                {
                    var factory = container.GetInstance<IFactory>();
                    var value = factory.GetSeveralPrimitives(42, true);
                    Assert.AreEqual(42, value.TheAnswer);
                    Assert.AreEqual(true, value.IsTrue);
                }
            }

            [Test]
            public void It_can_still_resolve_regular_pluginTypes_with_parameters()
            {
                var container = new Container(cfg =>
                                                {
                                                    cfg.For<IFactory>().CreateFactory();
                                                    cfg.For<IService>().Use<Service>();
                                                });
                using (container)
                {
                    var factory = container.GetInstance<IFactory>();
                    var value = factory.GetSeveralPrimitives(42, true);
                    Assert.AreEqual(42, value.TheAnswer);
                    Assert.AreEqual(true, value.IsTrue);
                }
            }
        }
    }
}