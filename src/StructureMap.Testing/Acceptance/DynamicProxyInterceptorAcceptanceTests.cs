using NUnit.Framework;
using Shouldly;
using StructureMap.DynamicInterception;
using System;
using System.Collections.Generic;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class DynamicProxyInterceptorAcceptanceTests
    {
        [TestCase(111, 10)]
        [TestCase(16, 4444)]
        [TestCase(-16, 4444)]
        public void CallAllSuccesfulInterceptors(int value, int expectedResult)
        {
            var container = new Container(x =>
            {
                x.For<IMathService>().Use<MathService>()
                    .InterceptWith(new DynamicProxyInterceptor<IMathService>(new IInterceptionBehavior[] { new NegatingInterceptor(), new CachingInterceptor() }));
            });

            var service = container.GetInstance<IMathService>();

            service.GetSquareRoot(value).ShouldBe(expectedResult);
        }

        public interface IMathService
        {
            int GetSquareRoot(int value);
        }

        private class MathService : IMathService
        {
            public int GetSquareRoot(int value)
            {
                return (int)Math.Sqrt(value);
            }
        }

        private class NegatingInterceptor : IInterceptionBehavior
        {
            public IMethodInvocationResult Intercept(IMethodInvocation methodInvocation)
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

        private class CachingInterceptor : IInterceptionBehavior
        {
            private static readonly IDictionary<int, int> PrecalculatedValues = new Dictionary<int, int>
            {
                { 16, 4444 },
                { 10, 5555 },
            };

            public IMethodInvocationResult Intercept(IMethodInvocation methodInvocation)
            {
                var argument = methodInvocation.GetArgument("value");
                var argumentValue = (int)argument.Value;

                int result;
                return PrecalculatedValues.TryGetValue(argumentValue, out result)
                    ? methodInvocation.CreateResult(result)
                    : methodInvocation.InvokeNext();
            }
        }
    }
}