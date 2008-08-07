using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

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
            _colorWidget = typeof (ColorWidget);
            _moneywidgetmaker = typeof (MoneyWidgetMaker);
        }

        #endregion

        private Plugin _plugin;
        private Type _iwidget;
        private Type _widgetmaker;
        private Type _colorWidget;
        private Type _moneywidgetmaker;

        private ParameterInfo param(string name)
        {
            Constructor ctor = new Constructor(typeof (LotsOfStuff));
            foreach (ParameterInfo parameterInfo in ctor.Ctor.GetParameters())
            {
                if (parameterInfo.Name == name)
                {
                    return parameterInfo;
                }
            }

            throw new NotImplementedException();
        }

        private PropertyInfo prop(string name)
        {
            return typeof (LotsOfStuff).GetProperty(name);
        }

        public class CanBeAutoFilledWithArray
        {
            public CanBeAutoFilledWithArray(IWidget[] widgets)
            {
            }

            [SetterProperty]
            public IWidget[] More
            {
                get { return null; }
                set { }
            }
        }

        public class GTO
        {
            private IEngine _engine;


            public GTO(string name, double price)
            {
            }

            [SetterProperty]
            public IEngine Engine
            {
                get { return _engine; }
                set { _engine = value; }
            }
        }


        [Test]
        public void BadPluginToAbstractClass()
        {
            Assert.AreEqual(false, TypeRules.CanBeCast(_widgetmaker, _colorWidget), "ColorWidget is NOT a WidgetMaker");
        }

        [Test]
        public void BadPluginToInterface()
        {
            Assert.AreEqual(false, TypeRules.CanBeCast(_iwidget, _moneywidgetmaker),
                            "MoneyWidgetMaker is NOT an IWidget");
        }

        [Test]
        public void CanBeAutoFilled_with_child_array_in_ctor()
        {
            Constructor ctor = new Constructor(typeof (CanBeAutoFilledWithArray));
            Assert.IsTrue(ctor.CanBeAutoFilled());
        }

        [Test]
        public void CanBeAutoFilled_with_child_array_in_setter()
        {
            SetterPropertyCollection setters =
                new SetterPropertyCollection(new Plugin(typeof (CanBeAutoFilledWithArray)));
            Assert.IsTrue(setters.CanBeAutoFilled());
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

            Container manager = new Container(pluginGraph);

            Mustang mustang = (Mustang) manager.GetInstance(typeof (IAutomobile), "Mustang");

            Assert.IsNotNull(mustang);
            Assert.IsTrue(mustang.Engine is PushrodEngine);
        }


        [Test]
        public void CanNotPluginWithoutAttribute()
        {
            string msg = "NotPluggableWidget cannot plug into IWidget automatically";
            Assert.AreEqual(false, TypeRules.IsExplicitlyMarkedAsPlugin(_iwidget, typeof (NotPluggable)), msg);
        }

        [Test]
        public void CanPluginWithAttribute()
        {
            Assert.AreEqual(true, TypeRules.IsExplicitlyMarkedAsPlugin(_iwidget, _colorWidget),
                            "ColorWidget plugs into IWidget");
        }


        [Test]
        public void CreateImplicitMementoWithNoConstructorArguments()
        {
            Plugin plugin = new Plugin(typeof (DefaultGateway), "Default");
            Assert.IsTrue(plugin.CanBeAutoFilled);

            IConfiguredInstance instance = (ConfiguredInstance) plugin.CreateImplicitInstance();

            Assert.AreEqual("Default", instance.Name);
            Assert.AreEqual(typeof (DefaultGateway), instance.PluggedType);
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
        public void DoesNotCreateAnImplicitMementoForAPluggedTypeThatCanBeAutoFilled()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void FindFirstConstructorArgumentOfType()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            string expected = "engine";

            string actual = plugin.FindArgumentNameForType<IEngine>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FindFirstConstructorArgumentOfType_in_a_setter_too()
        {
            Plugin plugin = new Plugin(typeof (GTO));

            Assert.AreEqual("Engine", plugin.FindArgumentNameForType<IEngine>());
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             ExpectedMessage =
                 "StructureMap Exception Code:  302\nThere is no argument of type StructureMap.Testing.Widget.IWidget for concrete type StructureMap.Testing.Graph.GrandPrix"
             )]
        public void FindFirstConstructorArgumentOfTypeNegativeCase()
        {
            Plugin plugin = new Plugin(typeof (GrandPrix));
            plugin.FindArgumentNameForType<IWidget>();
        }

        [Test]
        public void Get_concrete_key_from_attribute_if_it_exists()
        {
            Plugin plugin = new Plugin(typeof (ColorWidget));
            Assert.AreEqual("Color", plugin.ConcreteKey);
        }

        [Test]
        public void GetFirstMarkedConstructor()
        {
            Constructor ctor = new Constructor(typeof (ComplexRule));
            ConstructorInfo constructor = ctor.Ctor;

            Assert.IsNotNull(constructor);
            Assert.AreEqual(7, constructor.GetParameters().Length, "Should have 7 inputs, not 8");
        }

        [Test]
        public void GetGreediestConstructor()
        {
            Constructor ctor = new Constructor(typeof (GreaterThanRule));
            ConstructorInfo constructor = ctor.Ctor;

            Assert.IsNotNull(constructor);
            Assert.AreEqual(2, constructor.GetParameters().Length, "Should have 2 inputs");
        }


        [Test]
        public void GoodPluginToAbstractClass()
        {
            Assert.AreEqual(true, TypeRules.CanBeCast(_widgetmaker, _moneywidgetmaker),
                            "MoneyWidgetMaker is a WidgetMaker");
        }

        [Test]
        public void GoodPluginToInterface()
        {
            Assert.AreEqual(true, TypeRules.CanBeCast(_iwidget, _colorWidget), "ColorWidget is an IWidget");
        }

        [Test]
        public void ThrowGoodExceptionWhenNoPublicConstructorsFound()
        {
            try
            {
                Constructor ctor = new Constructor(typeof (ClassWithNoConstructor));
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
        public void Visit_arguments()
        {
            MockRepository mocks = new MockRepository();
            IArgumentVisitor visitor = mocks.CreateMock<IArgumentVisitor>();

            using (mocks.Record())
            {
                visitor.ChildParameter(param("engine"));
                visitor.ChildArrayParameter(param("engines"));
                visitor.StringParameter(param("name"));
                visitor.PrimitiveParameter(param("age"));
                visitor.EnumParameter(param("breed"));
                visitor.StringSetter(prop("LastName"), true);
                visitor.PrimitiveSetter(prop("Income"), true);
                visitor.ChildSetter(prop("Car"), true);
                visitor.ChildArraySetter(prop("Fleet"), true);
                visitor.EnumSetter(prop("OtherBreed"), true);
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
        private IAutomobile _car;
        private IAutomobile[] _fleet;
        private double _income;
        private string _lastName;
        private BreedEnum _otherBreed;

        public LotsOfStuff(IEngine engine, IEngine[] engines, string name, int age, BreedEnum breed)
        {
        }

        [SetterProperty]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        [SetterProperty]
        public double Income
        {
            get { return _income; }
            set { _income = value; }
        }

        [SetterProperty]
        public IAutomobile Car
        {
            get { return _car; }
            set { _car = value; }
        }

        [SetterProperty]
        public IAutomobile[] Fleet
        {
            get { return _fleet; }
            set { _fleet = value; }
        }

        [SetterProperty]
        public BreedEnum OtherBreed
        {
            get { return _otherBreed; }
            set { _otherBreed = value; }
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
        private readonly string _breed;
        private readonly IEngine _engine;
        private readonly int _horsePower;

        public GrandPrix(int horsePower, string breed, IEngine engine)
        {
            _horsePower = horsePower;
            _breed = breed;
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