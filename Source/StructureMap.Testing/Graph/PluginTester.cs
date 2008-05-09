using System;
using System.Drawing;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _plugin = new Plugin(typeof (ConfigurationWidget));
            _iwidget = typeof (IWidget);
            _widgetmaker = typeof (WidgetMaker);
            _colorwidget = typeof (ColorWidget);
            _moneywidgetmaker = typeof (MoneyWidgetMaker);
        }

        #endregion

        private Plugin _plugin;
        private Type _iwidget;
        private Type _widgetmaker;
        private Type _colorwidget;
        private Type _moneywidgetmaker;


        [Test]
        public void BadPluginToAbstractClass()
        {
            Assert.AreEqual(false, Plugin.CanBeCast(_widgetmaker, _colorwidget), "ColorWidget is NOT a WidgetMaker");
        }

        [Test]
        public void BadPluginToInterface()
        {
            Assert.AreEqual(false, Plugin.CanBeCast(_iwidget, _moneywidgetmaker), "MoneyWidgetMaker is NOT an IWidget");
        }

        [Test]
        public void Get_concrete_key_from_attribute_if_it_exists()
        {
            Plugin plugin = new Plugin(typeof(ColorWidget));
            Assert.AreEqual("Color", plugin.ConcreteKey);
        }

        [Test]
        public void CanBeAutoFilledIsFalse()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));

            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanBeAutoFilledIsTrue()
        {
            Plugin plugin = new Plugin(typeof (Mustang));

            Assert.IsTrue(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanCreateTheAutoFilledInstance()
        {
            // Builds a PluginGraph that includes all of the PluginFamily's and Plugin's 
            // defined in this file
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
            pluginGraph.Seal();

            InstanceManager manager = new InstanceManager(pluginGraph);

            Mustang mustang = (Mustang) manager.CreateInstance(typeof (IAutomobile), "Mustang");

            Assert.IsNotNull(mustang);
            Assert.IsTrue(mustang.Engine is PushrodEngine);
        }

        [Test]
        public void CanMakeObjectInstanceActivator()
        {
            Plugin plugin = new Plugin(typeof (DefaultGateway));
            Assert.IsTrue(!plugin.HasConstructorArguments(), "DefaultGateway can be just an activator");
        }

        [Test]
        public void CanNotMakeObjectInstanceActivator()
        {
            Plugin plugin = new Plugin(typeof (ComplexRule));
            Assert.IsTrue(plugin.HasConstructorArguments(), "ComplexRule cannot be just an activator");
        }

        [Test]
        public void CanNotPluginWithoutAttribute()
        {
            string msg = "NotPluggableWidget cannot plug into IWidget automatically";
            Assert.AreEqual(false, Plugin.IsExplicitlyMarkedAsPlugin(_iwidget, typeof (NotPluggable)), msg);
        }

        [Test]
        public void CanPluginWithAttribute()
        {
            Assert.AreEqual(true, Plugin.IsExplicitlyMarkedAsPlugin(_iwidget, _colorwidget), "ColorWidget plugs into IWidget");
        }


        [Test]
        public void CreateImplicitMementoWithNoConstructorArguments()
        {
            Plugin plugin = new Plugin(typeof (DefaultGateway), "Default");

            InstanceMemento memento = plugin.CreateImplicitMemento();
            Assert.IsNotNull(memento);
            Assert.AreEqual("Default", memento.InstanceKey);
            Assert.AreEqual("Default", memento.ConcreteKey);
        }

        [Test]
        public void CreateImplicitMementoWithSomeConstructorArgumentsReturnValueIsNull()
        {
            Plugin plugin = new Plugin(typeof (Strategy), "Default");
            InstanceMemento memento = plugin.CreateImplicitMemento();
            Assert.IsNull(memento);
        }

        [Test]
        public void CreateImplicitPluginSetsCorrectName()
        {
            Assert.AreEqual("Configuration", _plugin.ConcreteKey);
        }

        [Test]
        public void CreateImplicitPluginSetsCorrectType()
        {
            Assert.IsTrue(typeof (ConfigurationWidget).Equals(_plugin.PluggedType));
        }

        [Test]
        public void CreatePluginFromTypeThatDoesNotHaveAnAttributeDetermineTheConcreteKey()
        {
            Plugin plugin = new Plugin(GetType());
            Assert.AreEqual(GetType().AssemblyQualifiedName, plugin.ConcreteKey);
        }

        [Test]
        public void CreatesAnImplicitMementoForAPluggedTypeThatCanBeAutoFilled()
        {
            Plugin plugin = new Plugin(typeof (Mustang));
            InstanceMemento memento = plugin.CreateImplicitMemento();

            Assert.IsNotNull(memento);
            Assert.AreEqual(plugin.ConcreteKey, memento.InstanceKey);
            Assert.AreEqual(plugin.ConcreteKey, memento.ConcreteKey);
        }

        [Test]
        public void DoesNotCreateAnImplicitMementoForAPluggedTypeThatCanBeAutoFilled()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            InstanceMemento memento = plugin.CreateImplicitMemento();

            Assert.IsNull(memento);
        }

        [Test]
        public void FindFirstConstructorArgumentOfType()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            string expected = "engine";

            string actual = plugin.FindFirstConstructorArgumentOfType<IEngine>();
            Assert.AreEqual(expected, actual);
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
            ExpectedMessage = "StructureMap Exception Code:  302\nThere is no argument of type StructureMap.Testing.Widget.IWidget for concrete type StructureMap.Testing.Graph.GrandPrix"
             )]
        public void FindFirstConstructorArgumentOfTypeNegativeCase()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            plugin.FindFirstConstructorArgumentOfType<IWidget>();
        }

        [Test]
        public void GetFirstMarkedConstructor()
        {
            Plugin plugin = new Plugin(typeof (ComplexRule));
            ConstructorInfo constructor = plugin.GetConstructor();

            Assert.IsNotNull(constructor);
            Assert.AreEqual(7, constructor.GetParameters().Length, "Should have 7 inputs, not 8");
        }

        [Test]
        public void GetGreediestConstructor()
        {
            Plugin plugin = new Plugin(typeof (GreaterThanRule));
            ConstructorInfo constructor = plugin.GetConstructor();

            Assert.IsNotNull(constructor);
            Assert.AreEqual(2, constructor.GetParameters().Length, "Should have 2 inputs");
        }


        [Test]
        public void GoodPluginToAbstractClass()
        {
            Assert.AreEqual(true, Plugin.CanBeCast(_widgetmaker, _moneywidgetmaker), "MoneyWidgetMaker is a WidgetMaker");
        }

        [Test]
        public void GoodPluginToInterface()
        {
            Assert.AreEqual(true, Plugin.CanBeCast(_iwidget, _colorwidget), "ColorWidget is an IWidget");
        }

        [Test]
        public void ThrowGoodExceptionWhenNoPublicConstructorsFound()
        {
            try
            {
                Plugin plugin = new Plugin(typeof (ClassWithNoConstructor));
                plugin.GetConstructor();
                Assert.Fail("Should have thrown a StructureMapException");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(180, ex.ErrorCode);
                Assert.AreEqual(
                    "StructureMap Exception Code:  180\nCannot construct a Plugin for Class ClassWithNoConstructor, No public constructor found.",
                    ex.Message);
            }
        }

        [Test]
        public void ValidationMethods()
        {
            MemberInfo[] methods = _plugin.ValidationMethods;
            Assert.IsNotNull(methods);
            Assert.AreEqual(methods.Length, 2);
        }

        [Test]
        public void Visit_arguments()
        {
            MockRepository mocks = new MockRepository();
            IPluginArgumentVisitor visitor = mocks.CreateMock<IPluginArgumentVisitor>();

            using (mocks.Record())
            {
                visitor.Child("engine", typeof(IEngine));
                visitor.ChildArray("engines", typeof(IEngine));
                visitor.Primitive("name");
                visitor.Primitive("age");
                visitor.Primitive("color");
                visitor.Primitive("LastName");
                visitor.Primitive("Income");
                visitor.Child("Car", typeof(IAutomobile));
                visitor.ChildArray("Fleet", typeof(IAutomobile));
            }

            using (mocks.Playback())
            {
                Plugin plugin = new Plugin(typeof (LotsOfStuff));
                plugin.VisitArguments(visitor);
            }
        }
    }

    public class LotsOfStuff
    {
        private string _lastName;
        private double _income;
        private IAutomobile _car;
        private IAutomobile[] _fleet;

        public LotsOfStuff(IEngine engine, IEngine[] engines, string name, int age, Color color)
        {
            
        }

        [StructureMap.Attributes.SetterProperty]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        [StructureMap.Attributes.SetterProperty]
        public double Income
        {
            get { return _income; }
            set { _income = value; }
        }

        [StructureMap.Attributes.SetterProperty]
        public IAutomobile Car
        {
            get { return _car; }
            set { _car = value; }
        }

        [StructureMap.Attributes.SetterProperty]
        public IAutomobile[] Fleet
        {
            get { return _fleet; }
            set { _fleet = value; }
        }
    }

    [PluginFamily("Pushrod")]
    public interface IEngine
    {
    }

    [Pluggable("Pushrod")]
    public class PushrodEngine : IEngine
    {
    }

    [Pluggable("DOHC")]
    public class DOHCEngine : IEngine
    {
    }

    [PluginFamily]
    public interface IAutomobile
    {
    }

    [Pluggable("GrandPrix")]
    public class GrandPrix : IAutomobile
    {
        private readonly string _color;
        private readonly IEngine _engine;
        private readonly int _horsePower;

        public GrandPrix(int horsePower, string color, IEngine engine)
        {
            _horsePower = horsePower;
            _color = color;
            _engine = engine;
        }
    }

    [Pluggable("Mustang")]
    public class Mustang : IAutomobile
    {
        private readonly IEngine _engine;

        public Mustang(IEngine engine)
        {
            _engine = engine;
        }

        public IEngine Engine
        {
            get { return _engine; }
        }
    }

    [Pluggable("NoConstructor")]
    public class ClassWithNoConstructor
    {
        private ClassWithNoConstructor()
        {
        }
    }
}