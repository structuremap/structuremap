using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class explicit_arguments
    {
        [Test]
        public void supply_defaults_by_generic()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<BWidget>();
                x.For<IService>().Use<AService>();
            });

            var widget = new BWidget();
            var service = new BService();

            var guyWithWidgetAndService = container
                .With<IWidget>(widget)
                .With<IService>(service)
                .GetInstance<GuyWithWidgetAndService>();

            guyWithWidgetAndService
                .Widget.ShouldBeTheSameAs(widget);

            guyWithWidgetAndService
                .Service.ShouldBeTheSameAs(service);
        }

        [Test]
        public void supply_defaults_by_generic_in_a_bunch()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<BWidget>();
                x.For<IService>().Use<AService>();
            });

            var widget = new BWidget();
            var service = new BService();

            var guyWithWidgetAndService = container
                .With(x => {
                    x.With<IWidget>(widget);
                    x.With<IService>(service);
                })
                .GetInstance<GuyWithWidgetAndService>();

            guyWithWidgetAndService
                .Widget.ShouldBeTheSameAs(widget);

            guyWithWidgetAndService
                .Service.ShouldBeTheSameAs(service);
        }

        [Test]
        public void supply_named_arguments()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<ColorWidget>().Ctor<string>().Is("Red");
            });

            container.GetInstance<IWidget>()
                .ShouldBeOfType<ColorWidget>()
                .Color.ShouldEqual("Red");

            container.With("color").EqualTo("Blue")
                .GetInstance<IWidget>()
                .ShouldBeOfType<ColorWidget>()
                .Color.ShouldEqual("Blue");
        }
    }

    public class ColorWidget : IWidget
    {
        private readonly string _color;

        public ColorWidget(string color)
        {
            _color = color;
        }

        public string Color
        {
            get { return _color; }
        }
    }

    public class GuyWithWidgetAndService
    {
        private readonly IWidget _widget;
        private readonly IService _service;

        public GuyWithWidgetAndService(IWidget widget, IService service)
        {
            _widget = widget;
            _service = service;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }

        public IService Service
        {
            get { return _service; }
        }
    }
}