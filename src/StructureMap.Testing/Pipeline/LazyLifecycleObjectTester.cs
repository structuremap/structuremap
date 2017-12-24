using System;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class LazyLifecycleObjectTester
    {
        [Fact]
        public void when_initializing_then_value_is_not_created()
        {
            var mockValueFactory = Substitute.For<Func<object>>();
            var lazy = new LazyLifecycleObject<object>(mockValueFactory);

            lazy.IsValueCreated.ShouldBeFalse();
            lazy.ToString().ShouldBe("Value is not created.");
            mockValueFactory.Received(0).Invoke();
        }

        [Fact]
        public void when_requesting_value_then_value_is_created()
        {
            var mockValueFactory = Substitute.For<Func<object>>();
            var lazy = new LazyLifecycleObject<object>(mockValueFactory);
            var obj = new object();

            mockValueFactory.Invoke().Returns(obj);

            var value = lazy.Value;

            value.ShouldBeTheSameAs(obj);
            lazy.IsValueCreated.ShouldBeTrue();
            lazy.ToString().ShouldBe(obj.ToString());
            mockValueFactory.Received(1).Invoke();
        }

        [Fact]
        public void when_requesting_value_multiple_times_then_value_is_created_once()
        {
            var mockValueFactory = Substitute.For<Func<object>>();
            var lazy = new LazyLifecycleObject<object>(mockValueFactory);
            var obj = new object();

            mockValueFactory.Invoke().Returns(obj);

            var value1 = lazy.Value;
            var value2 = lazy.Value;

            value1.ShouldBeTheSameAs(obj);
            value2.ShouldBeTheSameAs(obj);
            lazy.IsValueCreated.ShouldBeTrue();
            mockValueFactory.Received(1).Invoke();
        }

        [Fact]
        public void when_requesting_value_on_multiple_threads_then_value_is_created_once()
        {
            var mockValueFactory = Substitute.For<Func<object>>();
            var lazy = new LazyLifecycleObject<object>(mockValueFactory);
            var obj = new object();

            mockValueFactory.Invoke().Returns(obj);

            object value1 = null;
            object value2 = null;

            Task.WaitAll(
                Task.Run(async () =>
                {
                    await Task.Yield();
                    value1 = lazy.Value;
                }),
                Task.Run(async () =>
                {
                    await Task.Yield();
                    value2 = lazy.Value;
                })
            );

            value1.ShouldBeTheSameAs(obj);
            value2.ShouldBeTheSameAs(obj);
            lazy.IsValueCreated.ShouldBeTrue();
            mockValueFactory.Received(1).Invoke();
        }
    }
}