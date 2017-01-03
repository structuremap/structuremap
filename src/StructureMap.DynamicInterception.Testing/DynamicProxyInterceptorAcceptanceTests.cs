using Shouldly;
using StructureMap.DynamicInterception;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StructureMap.DynamicInterception.Testing
{
    public class DynamicProxyInterceptorAcceptanceTests
    {
        [Theory]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Theory]
        [InlineData(111, 10)]
        [InlineData(16, 4444)]
        [InlineData(-16, 4444)]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithFailingSyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithFailingAsyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithFailingSyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithFailingAsyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [InlineData(-13)]
        public void CallSyncMethodWithThrowingSyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithThrowingAsyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithThrowingSyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithThrowingAsyncInterceptor(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithSyncThenFailingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithSyncThenFailingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithAsyncThenFailingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithAsyncThenFailingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithSyncThenFailingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithSyncThenFailingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithAsyncThenFailingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new ValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithAsyncThenFailingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new AsyncValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithSyncThenThrowingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithSyncThenThrowingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithAsyncThenThrowingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallSyncMethodWithAsyncThenThrowingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            Should.Throw<ArgumentException>(
                () => { service.GetSquareRoot(value); }
            );
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithSyncThenThrowingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithSyncThenThrowingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new CachingInterceptor(),
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithAsyncThenThrowingSyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new ThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Theory]
        [InlineData(-13)]
        public void CallAsyncMethodWithAsyncThenThrowingAsyncInterceptors(int value)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[]
                    {
                        new AsyncCachingInterceptor(),
                        new AsyncThrowingValidationInterceptor()
                    }));
            });

            var service = container.GetInstance<IMathService>();

            var task = service.GetSquareRootAsync(value);

            Should.Throw<ArgumentException>(() => task);
        }

        [Fact]
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
                await Task.Yield();
                return (int)Math.Sqrt(value);
            }
        }

        private class ValidationInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    return methodInvocation.CreateExceptionResult(new ArgumentException());
                }
                return methodInvocation.InvokeNext();
            }
        }

        private class AsyncValidationInterceptor : IAsyncInterceptionBehavior
        {
            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    return methodInvocation.CreateExceptionResult(new ArgumentException());
                }

                return await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
            }
        }

        private class ThrowingValidationInterceptor : ISyncInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(ISyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    throw new ArgumentException();
                }
                return methodInvocation.InvokeNext();
            }
        }

        private class AsyncThrowingValidationInterceptor : IAsyncInterceptionBehavior
        {
            public async Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;
                if (argumentValue < 0)
                {
                    throw new ArgumentException();
                }

                return await methodInvocation.InvokeNextAsync().ConfigureAwait(false);
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
                await Task.Yield();
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