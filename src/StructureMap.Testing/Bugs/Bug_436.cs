using Shouldly;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_436
    {
        [Fact]
        public void should_add_the_instance_name_when_created_with_type()
        {
            var container = new Container(_ =>
            {
                _.Policies.Interceptors(new DecoratorPolicy());

                _.For<IService>().MissingNamedInstanceIs.Type<FallbackService>();
            });

            var serviceName = "Bla";

            var decorator = container.GetInstance<IService>(new ExplicitArguments().Set(Guid.NewGuid()), serviceName) as DecoratorService;

            decorator.ShouldNotBeNull();
            decorator.Name.ShouldBe(serviceName);
            decorator.DecoratedService.ShouldBeOfType<FallbackService>();
        }

        private interface IService
        {
        }

        private class FallbackService : IService
        {
            public FallbackService(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; set; }
        }

        private class DecoratorService : IService
        {
            public DecoratorService(string name, IService decoratedService)
            {
                Name = name;
                DecoratedService = decoratedService;
            }

            public string Name { get; }

            public IService DecoratedService { get; }
        }

        private class DecoratorPolicy : IInterceptorPolicy
        {
            public string Description => "Test";

            public IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance)
            {
                if (pluginType == typeof(IService))
                {
                    yield return new FuncInterceptor<IService>((context, service) => new DecoratorService(instance.Name, service));
                }
            }
        }
    }
}