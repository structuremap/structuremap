using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;

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
            var ctor = new Constructor(typeof (LotsOfStuff));
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
            public IWidget[] More { get { return null; } set { } }
        }

        public class GTO
        {
            public GTO(string name, double price)
            {
            }

            [SetterProperty]
            public IEngine Engine { get; set; }
        }

        public class ClassWithProperties
        {
            public IEngine Engine { get; set; }
            public IAutomobile Car { get; set; }
            public IGateway Gateway { get; set; }
        }

        public class ClassWithNoConstructor
        {
            private ClassWithNoConstructor()
            {
            }
        }

        [Test]
        public void BadPluginToAbstractClass()
        {
            Assert.AreEqual(false, _colorWidget.CanBeCastTo(_widgetmaker), "ColorWidget is NOT a WidgetMaker");
        }

        [Test]
        public void BadPluginToInterface()
        {
            Assert.AreEqual(false, _moneywidgetmaker.CanBeCastTo(_iwidget),
                            "MoneyWidgetMaker is NOT an IWidget");
        }

        [Test]
        public void CanBeAutoFilled_with_child_array_in_ctor()
        {
            var ctor = new Constructor(typeof (CanBeAutoFilledWithArray));
            Assert.IsTrue(ctor.CanBeAutoFilled());
        }

        [Test]
        public void CanBeAutoFilled_with_child_array_in_setter()
        {
            var setters =
                new SetterPropertyCollection(new Plugin(typeof (CanBeAutoFilledWithArray)));
            Assert.IsTrue(setters.CanBeAutoFilled());
        }

        [Test]
        public void CanBeAutoFilledIsFalse()
        {
            var plugin = new Plugin(typeof (GrandPrix));

            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanBeAutoFilledIsTrue()
        {
            var plugin = new Plugin(typeof (Mustang));

            Assert.IsTrue(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanBeCreated_is_negative_with_no_public_constructors()
        {
            new Plugin(typeof (ClassWithNoConstructor)).CanBeCreated().ShouldBeFalse();
        }

        [Test]
        public void CanBeCreated_positive_with_a_public_constructor()
        {
            new Plugin(typeof (LotsOfStuff)).CanBeCreated().ShouldBeTrue();
        }

        [Test]
        public void CanCreateTheAutoFilledInstance()
        {
            // Builds a PluginGraph that includes all of the PluginFamily's and Plugin's 
            // defined in this file
            var pluginGraph = new PluginGraph();
            pluginGraph.Scan(x => x.Assembly(Assembly.GetExecutingAssembly()));
            pluginGraph.Seal();

            var manager = new Container(pluginGraph);

            var mustang = (Mustang) manager.GetInstance(typeof (IAutomobile), "Mustang");

            Assert.IsNotNull(mustang);
            Assert.IsTrue(mustang.Engine is PushrodEngine);
        }

        [Test]
        public void cannot_be_auto_filled_with_no_contructors()
        {
            var plugin = new Plugin(typeof (ClassWithNoConstructor));
            plugin.CanBeAutoFilled.ShouldBeFalse();
        }


        [Test]
        public void CanNotPluginWithoutAttribute()
        {
            Assert.IsFalse(typeof (NotPluggable).IsExplicitlyMarkedAsPlugin(_iwidget),
                           "NotPluggableWidget cannot plug into IWidget automatically");
        }

        [Test]
        public void CanPluginWithAttribute()
        {
            Assert.IsTrue(_colorWidget.IsExplicitlyMarkedAsPlugin(_iwidget), "ColorWidget plugs into IWidget");
        }


        [Test]
        public void CreateImplicitMementoWithNoConstructorArguments()
        {
            var plugin = new Plugin(typeof (DefaultGateway), "Default");
            Assert.IsTrue(plugin.CanBeAutoFilled);

            IConfiguredInstance instance = (ConfiguredInstance) plugin.CreateImplicitInstance();

            Assert.AreEqual("Default", instance.Name);
            Assert.AreEqual(typeof (DefaultGateway), instance.TPluggedType);
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
            var plugin = new Plugin(GetType());
            Assert.AreEqual(GetType().AssemblyQualifiedName, plugin.ConcreteKey);
        }

        [Test]
        public void DoesNotCreateAnImplicitMementoForATPluggedTypeThatCanBeAutoFilled()
        {
            var plugin = new Plugin(typeof (GrandPrix));
            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void find_argument_type_if_it_is_a_setter()
        {
            var plugin = new Plugin(typeof (GTO));
            plugin.FindArgumentType("Engine").ShouldEqual(typeof (IEngine));
        }

        [Test]
        public void find_argument_type_if_it_is_constructor()
        {
            var plugin = new Plugin(typeof (GrandPrix));
            plugin.FindArgumentType("engine").ShouldEqual(typeof (IEngine));
        }

        [Test]
        public void FindFirstConstructorArgumentOfType()
        {
            var plugin = new Plugin(typeof (GrandPrix));
            string expected = "engine";

            string actual = plugin.FindArgumentNameForType<IEngine>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FindFirstConstructorArgumentOfType_in_a_setter_too()
        {
            var plugin = new Plugin(typeof (GTO));

            Assert.AreEqual("Engine", plugin.FindArgumentNameForType<IEngine>());
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             ExpectedMessage =
                 "StructureMap Exception Code:  302\nThere is no argument of type StructureMap.Testing.Widget.IWidget for concrete type StructureMap.Testing.Graph.GrandPrix"
             )]
        public void FindFirstConstructorArgumentOfTypeNegativeCase()
        {
            var plugin = new Plugin(typeof (GrandPrix));
            plugin.FindArgumentNameForType<IWidget>();
        }

        [Test]
        public void Get_concrete_key_from_attribute_if_it_exists()
        {
            var plugin = new Plugin(typeof (ColorWidget));
            Assert.AreEqual("Color", plugin.ConcreteKey);
        }

        [Test]
        public void GetFirstMarkedConstructor()
        {
            var ctor = new Constructor(typeof (ComplexRule));
            ConstructorInfo constructor = ctor.Ctor;

            Assert.IsNotNull(constructor);
            Assert.AreEqual(7, constructor.GetParameters().Length, "Should have 7 inputs, not 8");
        }

        [Test]
        public void GetGreediestConstructor()
        {
            var ctor = new Constructor(typeof (GreaterThanRule));
            ConstructorInfo constructor = ctor.Ctor;

            Assert.IsNotNull(constructor);
            Assert.AreEqual(2, constructor.GetParameters().Length, "Should have 2 inputs");
        }


        [Test]
        public void GoodPluginToAbstractClass()
        {
            Assert.AreEqual(true, _moneywidgetmaker.CanBeCastTo(_widgetmaker),
                            "MoneyWidgetMaker is a WidgetMaker");
        }

        [Test]
        public void GoodPluginToInterface()
        {
            Assert.AreEqual(true, _colorWidget.CanBeCastTo(_iwidget), "ColorWidget is an IWidget");
        }

        [Test]
        public void SetFilledTypes_1()
        {
            PluginCache.ResetAll();
            PluginCache.AddFilledType(typeof (IEngine));
            PluginCache.AddFilledType(typeof (IAutomobile));

            Plugin plugin = PluginCache.GetPlugin(typeof (ClassWithProperties));

            plugin.Setters.IsMandatory("Engine").ShouldBeTrue();
            plugin.Setters.IsMandatory("Car").ShouldBeTrue();
            plugin.Setters.IsMandatory("Gateway").ShouldBeFalse();
        }


        [Test]
        public void SetFilledTypes_2()
        {
            PluginCache.ResetAll();
            PluginCache.AddFilledType(typeof (IGateway));
            PluginCache.AddFilledType(typeof (IAutomobile));

            Plugin plugin = PluginCache.GetPlugin(typeof (ClassWithProperties));

            plugin.Setters.IsMandatory("Engine").ShouldBeFalse();
            plugin.Setters.IsMandatory("Car").ShouldBeTrue();
            plugin.Setters.IsMandatory("Gateway").ShouldBeTrue();
        }


        [Test]
        public void SetFilledTypes_3()
        {
            PluginCache.ResetAll();
            PluginCache.AddFilledType(typeof (IGateway));

            Plugin plugin = PluginCache.GetPlugin(typeof (ClassWithProperties));

            plugin.Setters.IsMandatory("Engine").ShouldBeFalse();
            plugin.Setters.IsMandatory("Car").ShouldBeFalse();
            plugin.Setters.IsMandatory("Gateway").ShouldBeTrue();
        }

        [Test]
        public void ThrowGoodExceptionWhenNoPublicConstructorsFound()
        {
            try
            {
                new Registry().For<ClassWithNoConstructor>().Use<ClassWithNoConstructor>();
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(180, ex.ErrorCode);
            }
        }


        [Test]
        public void Visit_arguments()
        {
            var mocks = new MockRepository();
            var visitor = mocks.StrictMock<IArgumentVisitor>();

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
                var plugin = new Plugin(typeof (LotsOfStuff));
                plugin.VisitArguments(visitor);
            }
        }
    }


    [TestFixture]
    public class when_finding_property_name_of_enumerable_type
    {
        private Plugin plugin;



        [SetUp]
        public void SetUp()
        {
            plugin = new Plugin(typeof (ClassWithEnumerables));
        }

        [Test]
        public void smoke_test()
        {
            new Plugin(typeof (ClassThatUsesValidators)).FindArgumentNameForEnumerableOf(typeof (IValidator)).
                ShouldEqual("validators");
        }

        [Test]
        public void array_in_ctor()
        {
            plugin.FindArgumentNameForEnumerableOf(typeof (IEngine)).ShouldEqual("engines");
        }

        [Test]
        public void enumerable_in_ctor()
        {
            plugin.FindArgumentNameForEnumerableOf(typeof (IAutomobile)).ShouldEqual("autos");
        }

        [Test]
        public void ilist_as_setter()
        {
            plugin.FindArgumentNameForEnumerableOf(typeof (IWidget)).ShouldEqual("Widgets");
        }

        [Test]
        public void list_as_setter()
        {
            plugin.FindArgumentNameForEnumerableOf(typeof (Rule)).ShouldEqual("Rules");
        }
    }


    public class ClassWithEnumerables
    {
        public ClassWithEnumerables(IEngine[] engines, IEnumerable<IAutomobile> autos)
        {

        }

        public IList<IWidget> Widgets { get; set; }
        public List<Rule> Rules { get; set; }
    }

    public class LotsOfStuff
    {
        public LotsOfStuff(IEngine engine, IEngine[] engines, string name, int age, BreedEnum breed)
        {
        }

        [SetterProperty]
        public string LastName { get; set; }

        [SetterProperty]
        public double Income { get; set; }

        [SetterProperty]
        public IAutomobile Car { get; set; }

        [SetterProperty]
        public IAutomobile[] Fleet { get; set; }

        [SetterProperty]
        public BreedEnum OtherBreed { get; set; }
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

        public IEngine Engine { get { return _engine; } }
    }

    [Pluggable("NoConstructor")]
    public class ClassWithNoConstructor
    {
        private ClassWithNoConstructor()
        {
        }
    }
}