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
        public void ExceptionReturnsOriginalExceptionTypeForSyncMethod()
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
        public async Task ExceptionReturnsOriginalExceptionTypeForAsyncMethod()
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

        private class DummyInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.InvokeNext();
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
    }
}