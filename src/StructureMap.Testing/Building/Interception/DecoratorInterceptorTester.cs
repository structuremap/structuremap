using NUnit.Framework;
using Shouldly;
using StructureMap.Building.Interception;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class DecoratorInterceptorTester
    {
        [Test]
        public void accepts_type_is_the_plugin_type()
        {
            var interceptor = new DecoratorInterceptor(typeof (IWidget), typeof (WidgetDecorator));
            interceptor.Accepts.ShouldBe(typeof (IWidget));
        }

        [Test]
        public void role_is_decorator()
        {
            var interceptor = new DecoratorInterceptor(typeof(IWidget), typeof(WidgetDecorator));
            interceptor.Role.ShouldBe(InterceptorRole.Decorates);
        }

        [Test]
        public void integrated_test_with_the_expression()
        {
            var interceptor = new DecoratorInterceptor(typeof(IWidget), typeof(WidgetDecorator));

            var container = new Container(x => {
                x.For<IWidget>().Use<AWidget>().InterceptWith(interceptor);
            });

            container.GetInstance<IWidget>()
                .ShouldBeOfType<WidgetDecorator>()
                .Widget.ShouldBeOfType<AWidget>();
        }
    }

    public interface IWidget
    {
        void DoSomething();
    }

    public class AWidget : IWidget
    {
        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }

    public class WidgetDecorator : IWidget
    {
        private readonly IWidget _widget;

        public WidgetDecorator(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }

        public void DoSomething()
        {
            throw new System.NotImplementedException();
        }
    }
}