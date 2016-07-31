using Shouldly;
using StructureMap.Testing.Widget;
using System;
using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class AddInstanceTester
    {
        public AddInstanceTester()
        {
            container = new Container(registry =>
            {
                //registry.Scan(x => x.AssemblyContainingType<ColorWidget>());

                // Add an instance with properties
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("DarkGreen")
                    .Ctor<string>("color").Is("DarkGreen");

                // Add an instance by specifying the ConcreteKey
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("Purple")
                    .Ctor<string>("color").Is("Purple");

                registry.For<IWidget>().Add<AWidget>();
            });
        }

        private IContainer container;

        [Fact]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteKey()
        {
            var widget = (ColorWidget)container.GetInstance<IWidget>("Purple");
            widget.Color.ShouldBe("Purple");
        }

        [Fact]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteType()
        {
            var widget = (ColorWidget)container.GetInstance<IWidget>("DarkGreen");
            widget.Color.ShouldBe("DarkGreen");
        }

        [Fact]
        public void AddInstanceAndOverrideTheConcreteTypeForADependency()
        {
            IContainer container = new Container(x =>
            {
                x.For<Rule>().Add<WidgetRule>()
                    .Named("AWidgetRule")
                    .Ctor<IWidget>().IsSpecial(i => i.Type<AWidget>());
            });

            container.GetInstance<Rule>("AWidgetRule")
                .IsType<WidgetRule>()
                .Widget.IsType<AWidget>();
        }

        // SAMPLE: named-instance
        [Fact]
        public void SimpleCaseWithNamedInstance()
        {
            container = new Container(x => { x.For<IWidget>().Add<AWidget>().Named("MyInstance"); });
            // retrieve an instance by name
            var widget = (AWidget)container.GetInstance<IWidget>("MyInstance");
            widget.ShouldNotBeNull();
        }

        // ENDSAMPLE

        [Fact]
        public void SpecifyANewInstanceOverrideADependencyWithANamedInstance()
        {
            container = new Container(registry =>
            {
                registry.For<Rule>().Add<ARule>().Named("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("Purple")
                    .Ctor<string>("color").Is("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.For<Rule>().Add<WidgetRule>().Named("RuleThatUsesMyInstance")
                    .Ctor<IWidget>("widget").IsSpecial(x => x.TheInstanceNamed("Purple"));
            });

            container.GetInstance<Rule>("Alias").ShouldBeOfType<ARule>();

            var rule = (WidgetRule)container.GetInstance<Rule>("RuleThatUsesMyInstance");
            rule.Widget.As<ColorWidget>().Color.ShouldBe("Purple");
        }

        [Fact]
        public void SpecifyANewInstanceWithADependency()
        {
            // Specify a new Instance, create an instance for a dependency on the fly
            var instanceKey = "OrangeWidgetRule";

            var theContainer = new Container(registry =>
            {
                registry.For<Rule>().Add<WidgetRule>().Named(instanceKey)
                    .Ctor<IWidget>().IsSpecial(
                        i => i.Type<ColorWidget>().Ctor<string>("color").Is("Orange").Named("Orange"));
            });

            theContainer.GetInstance<Rule>(instanceKey).As<WidgetRule>()
                .Widget.As<ColorWidget>()
                .Color.ShouldBe("Orange");
        }

        [Fact]
        public void UseAPreBuiltObjectWithAName()
        {
            // Return the specific instance when an IWidget named "Lindsey" is requested
            var lindsey = new CloneableWidget("Lindsey");

            container =
                new Container(x => x.For<IWidget>().Add(lindsey).Named("Lindsey"));

            var widget1 = (CloneableWidget)container.GetInstance<IWidget>("Lindsey");
            var widget2 = (CloneableWidget)container.GetInstance<IWidget>("Lindsey");
            var widget3 = (CloneableWidget)container.GetInstance<IWidget>("Lindsey");

            lindsey.ShouldBeTheSameAs(widget1);
            lindsey.ShouldBeTheSameAs(widget2);
            lindsey.ShouldBeTheSameAs(widget3);
        }
    }

    public class WidgetRule : Rule
    {
        private readonly IWidget _widget;

        public WidgetRule(IWidget widget)
        {
            _widget = widget;
        }

        public IWidget Widget
        {
            get { return _widget; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var widgetRule = obj as WidgetRule;
            if (widgetRule == null) return false;
            return Equals(_widget, widgetRule._widget);
        }

        public override int GetHashCode()
        {
            return _widget != null ? _widget.GetHashCode() : 0;
        }
    }

    public class WidgetThing : IWidget
    {
        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion IWidget Members
    }

    public class CloneableWidget : IWidget
    {
        private readonly string _name;

        public CloneableWidget(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

    }

    public class ARule : Rule
    {
    }
}