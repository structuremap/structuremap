using NUnit.Framework;
using Shouldly;
using StructureMap.DynamicInterception;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class DynamicProxyInterceptorAcceptanceTests
    {
        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public void CallSyncMethodWithSyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new NegatingInterceptor(), new CachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(value).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public void CallSyncMethodWithAsyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncNegatingInterceptor(),
                        new AsyncCachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(value).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public void CallSyncMethodWithSyncThenAsyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new NegatingInterceptor(),
                        new AsyncCachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(value).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public void CallSyncMethodWithAsyncThenSyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncNegatingInterceptor(),
                        new CachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(value).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public async Task CallAsyncMethodWithSyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new NegatingInterceptor(),
                        new CachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            (await service.GetSquareRootAsync(value).ConfigureAwait(false)).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public async Task CallAsyncMethodWithAsyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncNegatingInterceptor(),
                        new AsyncCachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            (await service.GetSquareRootAsync(value).ConfigureAwait(false)).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public async Task CallAsyncMethodWithAsyncThenSyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncNegatingInterceptor(),
                        new CachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            (await service.GetSquareRootAsync(value).ConfigureAwait(false)).ShouldBe(expectedResult);
        }

        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public async Task CallAsyncMethodWithSyncThenAsyncInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new NegatingInterceptor(),
                        new AsyncCachingInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            (await service.GetSquareRootAsync(value).ConfigureAwait(false)).ShouldBe(expectedResult);
        }

        [Test]
        public void CallVoidSyncMethodWithSyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new VoidInterceptor(),
                        new VoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            service.DoNothing();
        }

        [Test]
        public void CallVoidSyncMethodWithAsyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new AsyncVoidInterceptor(),
                        new AsyncVoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            service.DoNothing();
        }

        [Test]
        public void CallVoidSyncMethodWithSyncThenAsyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new VoidInterceptor(),
                        new AsyncVoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            service.DoNothing();
        }

        [Test]
        public void CallVoidSyncMethodWithAsyncThenSyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new AsyncVoidInterceptor(),
                        new VoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            service.DoNothing();
        }

        [Test]
        public async Task CallVoidAsyncMethodWithSyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new VoidInterceptor(),
                        new VoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            await service.DoNothingAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CallVoidAsyncMethodWithAsyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new AsyncVoidInterceptor(),
                        new AsyncVoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            await service.DoNothingAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CallVoidAsyncMethodWithAsyncThenSyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new AsyncVoidInterceptor(),
                        new VoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            await service.DoNothingAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CallVoidAsyncMethodWithSyncThenAsyncInterceptors()
        {
            var container = new Container(x =>
            {
                x.For<IBusyService>().Use<BusyService>()
                    .InterceptWith(new DynamicProxyInterceptor<IBusyService>(new IInterceptionBehavior[]
                    {
                        new VoidInterceptor(),
                        new AsyncVoidInterceptor()
                    }));
            });

            var service = container.GetInstance<IBusyService>();

            await service.DoNothingAsync().ConfigureAwait(false);
        }

        [Test]
        public void UseInterceptionPolicy()
        {
            var container = new Container(x =>
            {
                x.Policies.Interceptors(new DynamicProxyInterceptorPolicy(new NegatingInterceptor(), new CachingInterceptor()));

                x.For<IMathService>().Use<MathService>();
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(-10).ShouldBe(5555);
        }

        public interface IMathService
        {
            int GetSquareRoot(int value);

            Task<int> GetSquareRootAsync(int value);
        }

        private class MathService : IMathService
        {
            public int GetSquareRoot(int value)
            {
                return (int)Math.Sqrt(value);
            }

            public async Task<int> GetSquareRootAsync(int value)
            {
                await Task.Delay(10).ConfigureAwait(false);
                return (int)Math.Sqrt(value);
            }
        }

        private class NegatingInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    argument.Value = -argumentValue;
                }
                return methodInvocation.InvokeNext();
            }
        }

        private class AsyncNegatingInterceptor : IAsyncInterceptionBehavior
        {
            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    argument.Value = -argumentValue;
                }
                return await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
            }
        }

        private class CachingInterceptor : ISyncInterceptionBehavior
        {
            private static readonly IDictionary<int, int> PrecalculatedValues = new Dictionary<int, int>
            {
                { 16, 4444 },
                { 10, 5555 },
            };

            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;

                int result;
                return PrecalculatedValues.TryGetValue(argumentValue, out result)
                    ? methodInvocation.CreateResult(result)
                    : methodInvocation.InvokeNext();
            }
        }

        private class AsyncCachingInterceptor : IAsyncInterceptionBehavior
        {
            private static readonly IDictionary<int, int> PrecalculatedValues = new Dictionary<int, int>
            {
                { 16, 4444 },
                { 10, 5555 },
            };

            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;

                int result;
                return PrecalculatedValues.TryGetValue(argumentValue, out result)
                    ? methodInvocation.CreateResult(result)
                    : await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
            }
        }

        public interface IBusyService
        {
            void DoNothing();

            Task DoNothingAsync();
        }

        private class BusyService : IBusyService
        {
            public void DoNothing()
            {
            }

            public async Task DoNothingAsync()
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
        }

        private class AsyncVoidInterceptor : IAsyncInterceptionBehavior
        {
            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                return await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
            }
        }

        private class VoidInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                return methodInvocation.InvokeNext();
            }
        }
    }
}