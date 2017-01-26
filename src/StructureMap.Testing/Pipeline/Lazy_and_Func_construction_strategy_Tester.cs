using Shouldly;
using System;
using Xunit;
using AWidget = StructureMap.Testing.Widget.AWidget;
using ColorWidget = StructureMap.Testing.Widget.ColorWidget;
using IWidget = StructureMap.Testing.Widget.IWidget;

namespace StructureMap.Testing.Pipeline
{
    public class Lazy_and_Func_construction_strategy_Tester
    {
        [Fact]
        public void FactoryTemplateTester()
        {
            var container =
                new Container();

            container.GetInstance<Func<ConcreteClass>>()().ShouldNotBeNull();
        }

        // SAMPLE: Lazy-in-usage
        public class WidgetLazyUser
        {
            private readonly Lazy<IWidget> _widget;

            public WidgetLazyUser(Lazy<IWidget> widget)
            {
                _widget = widget;
            }

            public IWidget Widget
            {
                get { return _widget.Value; }
            }
        }

        [Fact]
        public void lazy_resolution_in_action()
        {
            var container = new Container(_ =>
            {
                _.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<WidgetLazyUser>()
                .Widget.ShouldBeOfType<AWidget>();
        }

        // ENDSAMPLE

        [Fact]
        public void can_build_a_Lazy_of_T_automatically()
        {
            new Container().GetInstance<Lazy<ConcreteClass>>()
                .Value.ShouldBeOfType<ConcreteClass>().ShouldNotBeNull();
        }

        [Fact]
        public void build_a_func_for_a_concrete_class()
        {
            var container = new Container();
            var func = container.GetInstance<Func<ConcreteClass>>();

            func().ShouldNotBeNull();
        }

        [Fact]
        public void build_a_func_that_returns_a_transient()
        {
            var container =
                new Container(x => x.For<IWidget>().Use<ColorWidget>().Ctor<string>("color").Is("green"));

            var func = container.GetInstance<Func<IWidget>>();
            var w1 = func();
            var w2 = func();
            var w3 = func();

            w1.ShouldBeOfType<ColorWidget>().Color.ShouldBe("green");

            w1.ShouldNotBeTheSameAs(w2);
            w1.ShouldNotBeTheSameAs(w3);
            w2.ShouldNotBeTheSameAs(w3);
        }

        // SAMPLE: using-func-t
        [Fact]
        public void build_a_func_that_returns_a_singleton()
        {
            var container = new Container(x =>
            {
                x.ForSingletonOf<IWidget>().Use<ColorWidget>().Ctor<string>("color").Is("green");
            });

            var func = container.GetInstance<Func<IWidget>>();
            var w1 = func();
            var w2 = func();
            var w3 = func();

            w1.ShouldBeOfType<ColorWidget>().Color.ShouldBe("green");

            w1.ShouldBeTheSameAs(w2);
            w1.ShouldBeTheSameAs(w3);
            w2.ShouldBeTheSameAs(w3);
        }

        // ENDSAMPLE

        // SAMPLE: using-func-string-t
        [Fact]
        public void build_a_func_by_string()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("green").Named("green");
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("blue").Named("blue");
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("red").Named("red");
            });

            var func = container.GetInstance<Func<string, IWidget>>();
            func("green").ShouldBeOfType<ColorWidget>().Color.ShouldBe("green");
        }

        // ENDSAMPLE

        [Fact]
        public void build_a_func_by_string_from_default()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("green").Named("green");
                x.For<IWidget>().MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName));
            });

            var func = container.GetInstance<Func<string, IWidget>>();
            func("red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("red");
        }

        [Fact]
        public void build_a_func_by_string_from_parent()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("green").Named("green");
                x.For<IWidget>().MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName));
                x.ForConcreteType<ConcreteWidgetUser>();
            });

            var parent = container.GetInstance<ConcreteWidgetUser>();
            parent.Use("red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("red");
        }

        public class ConcreteWidgetUser
        {
            private readonly Func<string, IWidget> _widgetFactory;

            public ConcreteWidgetUser(Func<string, IWidget> widgetFactory)
            {
                _widgetFactory = widgetFactory;
            }

            public IWidget Use(string color)
            {
                return _widgetFactory(color);
            }
        }

        public class ConcreteClass
        {
        }

        // SAMPLE: using-lazy-as-workaround-for-bidirectional-dependency

        [Singleton]
        public class Thing1
        {
            private readonly Lazy<Thing2> _thing2;

            public Thing1(Lazy<Thing2> thing2)
            {
                _thing2 = thing2;
            }

            public Thing2 Thing2
            {
                get { return _thing2.Value; }
            }
        }

        [Singleton]
        public class Thing2
        {
            public Thing1 Thing1 { get; set; }

            public Thing2(Thing1 thing1)
            {
                Thing1 = thing1;
            }
        }

        [Fact]
        public void use_lazy_as_workaround_for_bi_directional_dependency()
        {
            var container = new Container();
            var thing1 = container.GetInstance<Thing1>();
            var thing2 = container.GetInstance<Thing2>();

            thing1.Thing2.ShouldBeSameAs(thing2);
            thing2.Thing1.ShouldBeSameAs(thing1);
        }

        // ENDSAMPLE
    }
}