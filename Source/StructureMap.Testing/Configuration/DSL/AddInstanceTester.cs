using System;
using System.Configuration;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
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
            
            manager = new InstanceManager(delegate(Registry registry)
            {
                registry.ScanAssemblies().IncludeAssemblyContainingType<ColorWidget>();

                // Add an instance with properties
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("DarkGreen")
                    .WithProperty("Color").EqualTo("DarkGreen");

                // Add an instance by specifying the ConcreteKey
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteTypeNamed("Color")
                    .WithName("Purple")
                    .WithProperty("Color").EqualTo("Purple");

                // Pull a property from the App config
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("AppSetting")
                    .WithProperty("Color").EqualToAppSetting("Color");


                registry.AddInstanceOf<IWidget>().UsingConcreteType<AWidget>();

            });
        }

        #endregion

        private IContainer manager;

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteKey()
        {
            ColorWidget widget = (ColorWidget) manager.GetInstance<IWidget>("Purple");
            Assert.AreEqual("Purple", widget.Color);
        }

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteType()
        {
            ColorWidget widget = (ColorWidget) manager.GetInstance<IWidget>("DarkGreen");
            Assert.AreEqual("DarkGreen", widget.Color);
        }

        [Test]
        public void AddInstanceAndOverrideTheConcreteTypeForADependency()
        {
            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                // Specify a new Instance that specifies the concrete type used for a dependency
                registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName("AWidgetRule")
                    .Child<IWidget>().IsConcreteType<AWidget>();
            });

            WidgetRule rule = (WidgetRule) manager.GetInstance<Rule>("AWidgetRule");
            Assert.IsInstanceOfType(typeof (AWidget), rule.Widget);
        }

        [Test, Explicit]
        public void CreateAnInstancePullAPropertyFromTheApplicationConfig()
        {
            Assert.AreEqual("Blue", ConfigurationManager.AppSettings["Color"]);
            ColorWidget widget = (ColorWidget) manager.GetInstance<IWidget>("AppSetting");
            Assert.AreEqual("Blue", widget.Color);
        }

        [Test]
        public void SimpleCaseWithNamedInstance()
        {
            manager = new InstanceManager(delegate(Registry registry)
            {
                // Specify a new Instance and override the Name
                registry.AddInstanceOf<IWidget>().UsingConcreteType<AWidget>().WithName("MyInstance");
            });

            AWidget widget = (AWidget) manager.GetInstance<IWidget>("MyInstance");
            Assert.IsNotNull(widget);
        }

        [Test]
        public void SpecifyANewInstanceOverrideADependencyWithANamedInstance()
        {
            manager = new InstanceManager(delegate(Registry registry)
            {
                registry.AddInstanceOf<Rule>().UsingConcreteType<ARule>().WithName("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.AddInstanceOf<IWidget>()
                    .UsingConcreteType<ColorWidget>()
                    .WithName("Purple")
                    .WithProperty("Color").EqualTo("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName("RuleThatUsesMyInstance")
                    .Child<IWidget>("widget").IsNamedInstance("Purple");
            });

            Assert.IsInstanceOfType(typeof (ARule), manager.GetInstance<Rule>("Alias"));

            WidgetRule rule = (WidgetRule) manager.GetInstance<Rule>("RuleThatUsesMyInstance");
            ColorWidget widget = (ColorWidget) rule.Widget;
            Assert.AreEqual("Purple", widget.Color);
        }

        [Test]
        public void SpecifyANewInstanceWithADependency()
        {
            // Specify a new Instance, create an instance for a dependency on the fly
            string instanceKey = "OrangeWidgetRule";

            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                registry.AddInstanceOf<Rule>().UsingConcreteType<WidgetRule>().WithName(instanceKey)
                    .Child<IWidget>().Is(
                    Registry.Instance<IWidget>().UsingConcreteType<ColorWidget>()
                        .WithProperty("Color").EqualTo("Orange")
                        .WithName("Orange")
                    );
            });


            WidgetRule rule = (WidgetRule) manager.GetInstance<Rule>(instanceKey);
            ColorWidget widget = (ColorWidget) rule.Widget;
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void UseAPreBuiltObjectForAnInstanceAsAPrototype()
        {
            // Build an instance for IWidget, then setup StructureMap to return cloned instances of the 
            // "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
            CloneableWidget theWidget = new CloneableWidget("Jeremy");
            

            manager = new InstanceManager(delegate(Registry registry)
            {
                registry.AddPrototypeInstanceOf<IWidget>(theWidget).WithName("Jeremy");
            });

            CloneableWidget widget1 = (CloneableWidget) manager.GetInstance<IWidget>("Jeremy");
            CloneableWidget widget2 = (CloneableWidget) manager.GetInstance<IWidget>("Jeremy");
            CloneableWidget widget3 = (CloneableWidget) manager.GetInstance<IWidget>("Jeremy");

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
            CloneableWidget julia = new CloneableWidget("Julia");

            manager = new InstanceManager(delegate(Registry registry)
            {
                registry.AddInstanceOf<IWidget>(julia).WithName("Julia");
            });

            CloneableWidget widget1 = (CloneableWidget) manager.GetInstance<IWidget>("Julia");
            CloneableWidget widget2 = (CloneableWidget) manager.GetInstance<IWidget>("Julia");
            CloneableWidget widget3 = (CloneableWidget) manager.GetInstance<IWidget>("Julia");

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
            WidgetRule widgetRule = obj as WidgetRule;
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
        private string _name;


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