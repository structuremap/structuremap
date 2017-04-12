using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StructureMap.DynamicInterception.Testing
{
    public class RetryInterceptorTests
    {
        [Theory]
        [InlineData(2)]
        public async Task RetryInterceptorDoesCorrectNumberOfRetries(int retries)
        {
            var container = new Container(x =>
            {
                x.For<ICounterService>().Use<CounterService>()
                    .InterceptWith(new DynamicProxyInterceptor<ICounterService>(new IInterceptionBehavior[]
                    {
                        new AsyncRetryInterceptor(retries)
                    }));
            });

            var service = container.GetInstance<ICounterService>();

            await service.IncrementAsync().ConfigureAwait(false);
            service.Counter.ShouldBe(retries);
        }

        public interface ICounterService
        {
            Task IncrementAsync();

            int Counter { get; set; }
        }

        private class CounterService : ICounterService
        {
            public Task IncrementAsync()
            {
                ++Counter;
                return Task.CompletedTask;
            }

            public int Counter { get; set; }
        }

        private class AsyncRetryInterceptor : IAsyncInterceptionBehavior
        {
            private readonly int _retries;
            private bool _invoked;

            public AsyncRetryInterceptor(int retries)
            {
                _retries = retries;
            }

            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                if (methodInvocation.InstanceMethodInfo.Name != "IncrementAsync")
                {
                    return await methodInvocation.InvokeNextAsync();
                }

                if (_invoked)
                {
                    throw new InvalidOperationException("Interceptor should be invoked once only");
                }
                _invoked = true;

                IMethodInvocationResult result = null;
                for (var i = 0; i < _retries; i++)
                {
                    result = await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
                    await Task.Delay(50).ConfigureAwait(false);
                }

                return result;
            }
        }
    }
}