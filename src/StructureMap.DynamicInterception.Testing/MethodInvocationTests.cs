using Shouldly;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace StructureMap.DynamicInterception.Testing
{
    public class MethodInvocationTests
    {
        [Fact]
        public void InstanceMethodInfoReturnsTargetImplementationMethodInfo()
        {
            var container = new Container(x =>
            {
                x.For<IFilteredService>().Use<FilteredService>()
                    .InterceptWith(new DynamicProxyInterceptor<IFilteredService>(new IInterceptionBehavior[]
                    {
                        new FilterInterceptor(), new DummyInterceptor()
                    }));
            });

            var service = container.GetInstance<IFilteredService>();

            service.IsIntercepted().ShouldBe(true);
            service.IsNotIntercepted().ShouldBe(false);
        }

        [Fact]
        public void ExceptionReturnsOriginalExceptionTypeForSyncMethodWithSyncInterceptor()
        {
            var container = new Container(x =>
            {
                x.For<IThrowingService>().Use<ThrowingService>()
                    .InterceptWith(new DynamicProxyInterceptor<IThrowingService>(new IInterceptionBehavior[]
                    {
                        new DummyInterceptor()
                    }));
            });

            var service = container.GetInstance<IThrowingService>();

            Should.Throw<InvalidOperationException>(() => service.Throw());
        }

        [Fact]
        public async Task ExceptionReturnsOriginalExceptionTypeForAsyncMethodWithSyncInterceptor()
        {
            var container = new Container(x =>
            {
                x.For<IThrowingService>().Use<ThrowingService>()
                    .InterceptWith(new DynamicProxyInterceptor<IThrowingService>(new IInterceptionBehavior[]
                    {
                        new DummyInterceptor()
                    }));
            });

            var service = container.GetInstance<IThrowingService>();

            await Should.ThrowAsync<InvalidOperationException>(() => service.ThrowAsync()).ConfigureAwait(false);
        }

        [Fact]
        public void ExceptionReturnsOriginalExceptionTypeForSyncMethodWithAsyncInterceptor()
        {
            var container = new Container(x =>
            {
                x.For<IThrowingService>().Use<ThrowingService>()
                    .InterceptWith(new DynamicProxyInterceptor<IThrowingService>(new IInterceptionBehavior[]
                    {
                        new DummyAsyncInterceptor()
                    }));
            });

            var service = container.GetInstance<IThrowingService>();
            Should.Throw<InvalidOperationException>(() => service.Throw());
        }

        [Fact]
        public async Task ExceptionReturnsOriginalExceptionTypeForAsyncMethodWithAsyncInterceptor()
        {
            var container = new Container(x =>
            {
                x.For<IThrowingService>().Use<ThrowingService>()
                    .InterceptWith(new DynamicProxyInterceptor<IThrowingService>(new IInterceptionBehavior[]
                    {
                        new DummyAsyncInterceptor()
                    }));
            });

            var service = container.GetInstance<IThrowingService>();

            await Should.ThrowAsync<InvalidOperationException>(() => service.ThrowAsync()).ConfigureAwait(false);
        }

        [Fact]
        public void ArgumentsReturnsCorrectParameterInfo()
        {
            var container = new Container(x =>
            {
                x.For<IFilteredParameterService>().Use<FilteredParameterService>()
                    .InterceptWith(new DynamicProxyInterceptor<IFilteredParameterService>(new IInterceptionBehavior[]
                    {
                        new FilterInterfaceParameterInterceptor(), new DummyInterceptor()
                    }));
            });

            var service = container.GetInstance<IFilteredParameterService>();

            service.IsInterceptedInInterface(0).ShouldBe(true);
            service.IsInterceptedInImplementation(0).ShouldBe(false);
        }

        [Fact]
        public void ArgumentsReturnsCorrectInstanceParameterInfo()
        {
            var container = new Container(x =>
            {
                x.For<IFilteredParameterService>().Use<FilteredParameterService>()
                    .InterceptWith(new DynamicProxyInterceptor<IFilteredParameterService>(new IInterceptionBehavior[]
                    {
                        new FilterImplementationParameterInterceptor(), new DummyInterceptor()
                    }));
            });

            var service = container.GetInstance<IFilteredParameterService>();

            service.IsInterceptedInInterface(0).ShouldBe(false);
            service.IsInterceptedInImplementation(0).ShouldBe(true);
        }

        private class DummyInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.InvokeNext();
            }
        }

        private class DummyAsyncInterceptor : IAsyncInterceptionBehavior
        {
            public Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                return methodInvocation.InvokeNextAsync();
            }
        }

        private class FilterInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.InstanceMethodInfo.GetCustomAttribute<FilterAttribute>() != null
                    ? methodInvocation.CreateResult(true)
                    : methodInvocation.InvokeNext();
            }
        }

        public interface IFilteredService
        {
            bool IsIntercepted();

            bool IsNotIntercepted();
        }

        private class FilteredService : IFilteredService
        {
            [Filter]
            public bool IsIntercepted()
            {
                return false;
            }

            public bool IsNotIntercepted()
            {
                return false;
            }
        }

        private class FilterAttribute : Attribute
        {
        }

        public interface IThrowingService
        {
            void Throw();

            Task ThrowAsync();
        }

        private class ThrowingService : IThrowingService
        {
            public void Throw()
            {
                throw new InvalidOperationException();
            }

            public async Task ThrowAsync()
            {
                throw new InvalidOperationException();
            }
        }

        public interface IFilteredParameterService
        {
            bool IsInterceptedInInterface([Filter]int value);

            bool IsInterceptedInImplementation(int value);
        }

        private class FilteredParameterService : IFilteredParameterService
        {
            public bool IsInterceptedInInterface(int value)
            {
                return false;
            }

            public bool IsInterceptedInImplementation([Filter]int value)
            {
                return false;
            }
        }

        private class FilterInterfaceParameterInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.Arguments[0].ParameterInfo.GetCustomAttribute<FilterAttribute>() != null
                    ? methodInvocation.CreateResult(true)
                    : methodInvocation.InvokeNext();
            }
        }

        private class FilterImplementationParameterInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.Arguments[0].InstanceParameterInfo.GetCustomAttribute<FilterAttribute>() != null
                    ? methodInvocation.CreateResult(true)
                    : methodInvocation.InvokeNext();
            }
        }
    }
}