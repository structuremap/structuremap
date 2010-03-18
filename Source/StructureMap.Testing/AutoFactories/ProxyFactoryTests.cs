using Castle.DynamicProxy;
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

            context.Stub(x => x.GetInstance<IService>()).Return(service);

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context);

            var factory = proxyFactory.Create();

            var built = factory.CreateService();

            built.ShouldNotBeNull();
            built.ShouldBeTheSameAs(service);
        }
    }
}