using Castle.DynamicProxy;
using Rhino.Mocks;
using StructureMap.AutoFactory;
using StructureMap.Pipeline;
using System;
using Xunit;

namespace StructureMap.Testing.AutoFactories
{
    public class ProxyFactoryTests
    {
        public interface IFactory
        {
            IService CreateService();
        }

        public interface IService
        {
        }

        public class Service : IService
        {
        }

        private static TDependency Stub<TDependency>() where TDependency : class
        {
            return MockRepository.GenerateStub<TDependency>();
        }

        [Fact]
        public void Should_create_a_proxy_factory()
        {
            var proxyGenerator = new ProxyGenerator();
            var context = Stub<IContext>();

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context, new DefaultAutoFactoryConventionProvider());

            var factory = proxyFactory.Create();

            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Should_build_the_service_by_return_type_from_context()
        {
            var proxyGenerator = new ProxyGenerator();
            var context = Stub<IContext>();
            var container = Stub<IContainer>();

            var service = new Service();

            context.Stub(x => x.GetInstance<IContainer>()).Return(container);
            container.Stub(x => x.TryGetInstance(Arg<Type>.Is.Same(typeof(IService)), Arg<ExplicitArguments>.Is.Anything)).Return(service);

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context, new DefaultAutoFactoryConventionProvider());

            var factory = proxyFactory.Create();

            var built = factory.CreateService();

            built.ShouldNotBeNull();
            built.ShouldBeTheSameAs(service);
        }
    }
}