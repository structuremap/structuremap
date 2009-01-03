using System;
using System.Configuration;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class AddInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(registry =>
            {
                registry.Scan(x => x.AssemblyContainingType<ColorWidget>());

                // Add an instance with properties
                registry.InstanceOf<IWidget>()
                    .Is.OfConcreteType<ColorWidget>()
                    .WithName("DarkGreen")
                    .WithProperty("color").EqualTo("DarkGreen");

                // Add an instance by specifying the ConcreteKey
                registry.InstanceOf<IWidget>()
                    .Is.OfConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("color").EqualTo("Purple");

                // Pull a property from the App config
                registry.InstanceOf<IWidget>()
                    .Is.OfConcreteType<ColorWidget>()
                    .WithName("AppSetting")
                    .WithProperty("color").EqualToAppSetting("Color");

                // Pull a property from the App config
                registry.InstanceOf<IWidget>()
                    .Is.OfConcreteType<NotPluggableWidget>()
                    .WithName("UsesDefaultValue")
                    .WithProperty("name").EqualToAppSetting("WidgetName", "TheDefaultValue");


                registry.InstanceOf<IWidget>().Is.OfConcreteType<AWidget>();
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
            IContainer container = new Container(
                registry => registry.InstanceOf<Rule>().Is.OfConcreteType<WidgetRule>()
                                .WithName("AWidgetRule")
                                .CtorDependency<IWidget>().Is(i => i.OfConcreteType<AWidget>()));

            container.GetInstance<Rule>("AWidgetRule")
                .IsType<WidgetRule>()
                .Widget.IsType<AWidget>();
        }

        [Test]
        public void CreateAnInstancePullAPropertyFromTheApplicationConfig()
        {
            Assert.AreEqual("Blue", ConfigurationManager.AppSettings["Color"]);
            var widget = (ColorWidget) container.GetInstance<IWidget>("AppSetting");
            Assert.AreEqual("Blue", widget.Color);
        }

        [Test]
        public void CreateAnInstanceUsingDefaultPropertyValueWhenSettingDoesNotExistInApplicationConfig()
        {
            Assert.AreEqual(null, ConfigurationManager.AppSettings["WidgetName"]);
            var widget = (NotPluggableWidget) container.GetInstance<IWidget>("UsesDefaultValue");
            Assert.AreEqual("TheDefaultValue", widget.Name);
        }

        [Test]
        public void SimpleCaseWithNamedInstance()
        {
            container =
                new Container(
                    registry => { registry.InstanceOf<IWidget>().Is.OfConcreteType<AWidget>().WithName("MyInstance"); });

            var widget = (AWidget) container.GetInstance<IWidget>("MyInstance");
            Assert.IsNotNull(widget);
        }


        [Test]
        public void SpecifyANewInstanceOverrideADependencyWithANamedInstance()
        {
            container = new Container(registry =>
            {
                registry.InstanceOf<Rule>().Is.OfConcreteType<ARule>().WithName("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.InstanceOf<IWidget>()
                    .Is.OfConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("color").EqualTo("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.InstanceOf<Rule>().Is.OfConcreteType<WidgetRule>().WithName("RuleThatUsesMyInstance")
                    .CtorDependency<IWidget>("widget").Is(x => x.TheInstanceNamed("Purple"));
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

            var theContainer = new Container(registry =>
            {
                registry.InstanceOf<Rule>().Is.OfConcreteType<WidgetRule>().WithName(instanceKey)
                    .CtorDependency<IWidget>().Is(
                    i => { i.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Orange").WithName("Orange"); });
            });

            var rule = (WidgetRule) theContainer.GetInstance<Rule>(instanceKey);
            var widget = (ColorWidget) rule.Widget;
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void UseAPreBuiltObjectForAnInstanceAsAPrototype()
        {
            // Build an instance for IWidget, then setup StructureMap to return cloned instances of the 
            // "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
            var theWidget = new CloneableWidget("Jeremy");

            container = new Container(x =>
            {
                x.InstanceOf<IWidget>().Is.PrototypeOf(theWidget).WithName("Jeremy");
            });

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
        public void UseAPreBuiltObjectForAnInstanceAsASerializedCopy()
        {
            // Build an instance for IWidget, then setup StructureMap to return cloned instances of the 
            // "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
            var theWidget = new CloneableWidget("Jeremy");

            container = new Container(x =>
            {
                x.InstanceOf<IWidget>().Is.SerializedCopyOf(theWidget).WithName("Jeremy");
            });

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
                new Container(registry => registry.InstanceOf<IWidget>().Is.Object(julia).WithName("Julia"));

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

    [Serializable]
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