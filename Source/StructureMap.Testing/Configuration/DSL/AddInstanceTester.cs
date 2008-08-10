using System;
using System.Configuration;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class AddInstanceTester : RegistryExpressions
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(registry =>
            {
                registry.ScanAssemblies().IncludeAssemblyContainingType<ColorWidget>();

                // Add an instance with properties
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("DarkGreen")
                    .WithProperty("color").EqualTo("DarkGreen");

                // Add an instance by specifying the ConcreteKey
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("color").EqualTo("Purple");

                // Pull a property from the App config
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("AppSetting")
                    .WithProperty("color").EqualToAppSetting("Color");


                registry.AddInstanceOf<IWidget>().UsingConcreteType<AWidget>();
            });
        }

        #endregion

        private IContainer container;

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteKey()
        {
            var widget = (ColorWidget) container.GetInstance<IWidget>("Purple");
            Assert.AreEqual("Purple", widget.Color);
        }

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteType()
        {
            var widget = (ColorWidget) container.GetInstance<IWidget>("DarkGreen");
            Assert.AreEqual("DarkGreen", widget.Color);
        }

        [Test]
        public void AddInstanceAndOverrideTheConcreteTypeForADependency()
        {
            IContainer manager = new Container(
                registry => registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName("AWidgetRule")
                                .Child<IWidget>().IsConcreteType<AWidget>());

            var rule = (WidgetRule) manager.GetInstance<Rule>("AWidgetRule");
            Assert.IsInstanceOfType(typeof (AWidget), rule.Widget);
        }

        [Test, Explicit]
        public void CreateAnInstancePullAPropertyFromTheApplicationConfig()
        {
            Assert.AreEqual("Blue", ConfigurationManager.AppSettings["Color"]);
            var widget = (ColorWidget) container.GetInstance<IWidget>("AppSetting");
            Assert.AreEqual("Blue", widget.Color);
        }

        [Test]
        public void SimpleCaseWithNamedInstance()
        {
            container = new Container(
                registry => registry.AddInstanceOf<IWidget>().UsingConcreteType<AWidget>().WithName("MyInstance"));

            var widget = (AWidget) container.GetInstance<IWidget>("MyInstance");
            Assert.IsNotNull(widget);
        }

        [Test]
        public void temp()
        {
            IContainer container = new Container(registry =>
            {
                registry.AddInstanceOf<Rule>().UsingConcreteType<ARule>().WithName("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("color").EqualTo("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName("RuleThatUsesMyInstance")
                    .Child<IWidget>("widget").IsNamedInstance("Purple");
            });
        }

        [Test]
        public void SpecifyANewInstanceOverrideADependencyWithANamedInstance()
        {
            container = new Container(registry =>
            {
                registry.AddInstanceOf<Rule>().UsingConcreteType<ARule>().WithName("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("color").EqualTo("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName("RuleThatUsesMyInstance")
                    .Child<IWidget>("widget").IsNamedInstance("Purple");
            });

            Assert.IsInstanceOfType(typeof (ARule), container.GetInstance<Rule>("Alias"));

            var rule = (WidgetRule) container.GetInstance<Rule>("RuleThatUsesMyInstance");
            var widget = (ColorWidget) rule.Widget;
            Assert.AreEqual("Purple", widget.Color);
        }

        [Test]
        public void SpecifyANewInstanceWithADependency()
        {
            // Specify a new Instance, create an instance for a dependency on the fly
            string instanceKey = "OrangeWidgetRule";

            IContainer manager = new Container(
                registry => registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName(instanceKey)
                                .Child<IWidget>().Is(
                                Instance<ColorWidget>()
                                    .WithProperty("color").EqualTo("Orange")
                                    .WithName("Orange")
                                ));


            var rule = (WidgetRule) manager.GetInstance<Rule>(instanceKey);
            var widget = (ColorWidget) rule.Widget;
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void UseAPreBuiltObjectForAnInstanceAsAPrototype()
        {
            // Build an instance for IWidget, then setup StructureMap to return cloned instances of the 
            // "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
            var theWidget = new CloneableWidget("Jeremy");


            container =
                new Container(
                    registry => registry.AddPrototypeInstanceOf<IWidget>(theWidget).WithName("Jeremy"));

            var widget1 = (CloneableWidget) container.GetInstance<IWidget>("Jeremy");
            var widget2 = (CloneableWidget) container.GetInstance<IWidget>("Jeremy");
            var widget3 = (CloneableWidget) container.GetInstance<IWidget>("Jeremy");

            Assert.AreEqual("Jeremy", widget1.Name);
            Assert.AreEqual("Jeremy", widget2.Name);
            Assert.AreEqual("Jeremy", widget3.Name);

            Assert.AreNotSame(widget1, widget2);
            Assert.AreNotSame(widget1, widget3);
            Assert.AreNotSame(widget2, widget3);
        }


        [Test]
        public void UseAPreBuiltObjectWithAName()
        {
            // Return the specific instance when an IWidget named "Julia" is requested
            var julia = new CloneableWidget("Julia");

            container =
                new Container(registry => registry.AddInstanceOf<IWidget>(julia).WithName("Julia"));

            var widget1 = (CloneableWidget) container.GetInstance<IWidget>("Julia");
            var widget2 = (CloneableWidget) container.GetInstance<IWidget>("Julia");
            var widget3 = (CloneableWidget) container.GetInstance<IWidget>("Julia");

            Assert.AreSame(julia, widget1);
            Assert.AreSame(julia, widget2);
            Assert.AreSame(julia, widget3);
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

        #endregion
    }

    public class CloneableWidget : IWidget, ICloneable
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

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ARule : Rule
    {
    }
}