using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class DecoratorPolicyTester
    {
        [Test]
        public void decorate_closed_type_no_filter_matches_on_plugin_type()
        {
            var policy = new DecoratorPolicy(typeof (IWidget), typeof (WidgetDecorator));

            policy.DetermineInterceptors(typeof (IWidget), new SmartInstance<AWidget>())
                .Single()
                .ShouldBeOfType<DecoratorInterceptor>()
                .Instance
                .PluggedType
                .ShouldBe(typeof (WidgetDecorator));

        }

        [Test]
        public void decorate_closed_type_no_filter_does_not_match_on_plugin_type()
        {
            var policy = new DecoratorPolicy(typeof(IWidget), typeof(WidgetDecorator));

            policy.DetermineInterceptors(typeof(AWidget), new SmartInstance<AWidget>())
                .Any().ShouldBeFalse();

        }

        [Test]
        public void decorate_closed_type_uses_filter()
        {
            var policy = new DecoratorPolicy(typeof(IWidget), typeof(WidgetDecorator), i => i.Name == "ok");

            policy.DetermineInterceptors(typeof(IWidget), new SmartInstance<AWidget>().Named("not right"))
                .Any().ShouldBeFalse();

            policy.DetermineInterceptors(typeof(IWidget), new SmartInstance<AWidget>().Named("ok"))
                .Any().ShouldBeTrue();
        }

        [Test]
        public void open_generics_happy_path()
        {
            var policy = new DecoratorPolicy(typeof (IFoo<,>), typeof (DecoratedFoo<,>));

            policy.DetermineInterceptors(typeof(IFoo<string, int>), new SmartInstance<Foo<string, int>>())
                .Single()
                .ShouldBeOfType<DecoratorInterceptor>()
                .Instance.PluggedType
                .ShouldBe(typeof(DecoratedFoo<string, int>));
        }
    }

    

    public interface IFoo<T1, T2>
    {
        T1 One { get; }
        T2 Two { get; }
    }

    public class Foo<T1, T2> : IFoo<T1, T2>
    {
        private readonly T1 _one;
        private readonly T2 _two;

        public Foo(T1 one, T2 two)
        {
            _one = one;
            _two = two;
        }

        public T1 One
        {
            get { return _one; }
        }

        public T2 Two
        {
            get { return _two; }
        }
    }

    public class DecoratedFoo<T1, T2> : IFoo<T1, T2>
    {
        private readonly IFoo<T1, T2> _inner;

        public DecoratedFoo(IFoo<T1, T2> inner)
        {
            _inner = inner;
        }

        public T1 One
        {
            get
            {
                return _inner.One;
            }
        }

        public T2 Two
        {
            get
            {
                return _inner.Two;
            }
        }
    }
}