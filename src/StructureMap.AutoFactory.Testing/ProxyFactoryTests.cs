using Castle.DynamicProxy;
using Moq;
using Shouldly;
using StructureMap.AutoFactory;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.AutoFactory.Testing
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

        [Fact]
        public void Should_create_a_proxy_factory()
        {
            var proxyGenerator = new ProxyGenerator();
            var context = new Mock<IContext>().Object;

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, context, new DefaultAutoFactoryConventionProvider());

            var factory = proxyFactory.Create();

            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Should_build_the_service_by_return_type_from_context()
        {
            var proxyGenerator = new ProxyGenerator();
            var contextMock = new Mock<IContext>();
            var containerMock = new Mock<IContainer>();

            var service = new Service();

            contextMock.Setup(x => x.GetInstance<IContainer>()).Returns(containerMock.Object);
            containerMock.Setup(x => x.TryGetInstance(typeof(IService), It.IsAny<ExplicitArguments>())).Returns(service);

            var proxyFactory = new ProxyFactory<IFactory>(proxyGenerator, contextMock.Object, new DefaultAutoFactoryConventionProvider());

            var factory = proxyFactory.Create();

            var built = factory.CreateService();

            built.ShouldNotBeNull();
            built.ShouldBe(service);
        }
    }
}