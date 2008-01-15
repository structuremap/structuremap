using System;
using System.Reflection;
using NUnit.Framework;
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
            _plugin = Plugin.CreateImplicitPlugin(typeof (ConfigurationWidget));
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

        [
            Test,
            ExpectedException(typeof (StructureMapException),
                "StructureMap Exception Code:  112\nMissing a mandatory \"ConcreteKey\" attribute in a <Plugin> node for Type \"StructureMap.Testing.Widget.NotPluggableWidget\""
                )]
        public void AddAPluggedTypeWithoutAConcreteKey()
        {
            TypePath path = new TypePath("StructureMap.Testing.Widget",
                                         "StructureMap.Testing.Widget.NotPluggableWidget");

            Plugin plugin = new Plugin(path, "");
        }

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
        public void CanBeAutoFilledIsFalse()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GrandPrix));

            Assert.IsFalse(plugin.CanBeAutoFilled);
        }

        [Test]
        public void CanBeAutoFilledIsTrue()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (Mustang));

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
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (DefaultGateway));
            Assert.IsTrue(!plugin.HasConstructorArguments(), "DefaultGateway can be just an activator");
        }

        [Test]
        public void CanNotMakeObjectInstanceActivator()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ComplexRule));
            Assert.IsTrue(plugin.HasConstructorArguments(), "ComplexRule cannot be just an activator");
        }

        [Test]
        public void CanNotPluginWithoutAttribute()
        {
            string msg = "NotPluggableWidget cannot plug into IWidget automatically";
            Assert.AreEqual(false, Plugin.IsAnExplicitPlugin(_iwidget, typeof (NotPluggable)), msg);
        }

        [Test]
        public void CanPluginWithAttribute()
        {
            Assert.AreEqual(true, Plugin.IsAnExplicitPlugin(_iwidget, _colorwidget), "ColorWidget plugs into IWidget");
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void CreateAPluginWithANonExistentAssembly()
        {
            TypePath path = new TypePath("IDontExist.Assembly",
                                         "IDontExist.Assembly.NotPluggableWidget");

            Plugin plugin = new Plugin(path, "default");
        }


        [
            Test,
            ExpectedException(typeof (StructureMapException))]
        public void CreateAPluginWithANonExistentClass()
        {
            TypePath path = new TypePath("StructureMap.Testing.Widget",
                                         "StructureMap.Testing.Widget.NonExistentClass");

            Plugin plugin = new Plugin(path, "default");
        }


        [Test]
        public void CreateImplicitMementoWithNoConstructorArguments()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (DefaultGateway), "Default", string.Empty);

            InstanceMemento memento = plugin.CreateImplicitMemento();
            Assert.IsNotNull(memento);
            Assert.AreEqual("Default", memento.InstanceKey);
            Assert.AreEqual("Default", memento.ConcreteKey);
            Assert.AreEqual(DefinitionSource.Implicit, memento.DefinitionSource);
        }

        [Test]
        public void CreateImplicitMementoWithSomeConstructorArgumentsReturnValueIsNull()
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(typeof (Strategy), "Default", string.Empty);
            InstanceMemento memento = plugin.CreateImplicitMemento();
            Assert.IsNull(memento);
        }

        [Test]
        public void CreateImplicitPluginDefinitionSourceIsImplicity()
        {
            Assert.AreEqual(DefinitionSource.Implicit, _plugin.DefinitionSource);
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
            Plugin plugin = Plugin.CreateImplicitPlugin(GetType());
            Assert.AreEqual(TypePath.GetAssemblyQualifiedName(GetType()), plugin.ConcreteKey);
        }

        [Test]
        public void CreatesAnImplicitMementoForAPluggedTypeThatCanBeAutoFilled()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (Mustang));
            InstanceMemento memento = plugin.CreateImplicitMemento();

            Assert.IsNotNull(memento);
            Assert.AreEqual(plugin.ConcreteKey, memento.InstanceKey);
            Assert.AreEqual(plugin.ConcreteKey, memento.ConcreteKey);
        }

        [Test]
        public void DoesNotCreateAnImplicitMementoForAPluggedTypeThatCanBeAutoFilled()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GrandPrix));
            InstanceMemento memento = plugin.CreateImplicitMemento();

            Assert.IsNull(memento);
        }

        [Test]
        public void FindFirstConstructorArgumentOfType()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GrandPrix));
            string expected = "engine";

            string actual = plugin.FindFirstConstructorArgumentOfType<IEngine>();
            Assert.AreEqual(expected, actual);
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  302\nThere is no argument of type StructureMap.Testing.Widget.IWidget for concrete type StructureMap.Testing.Graph.GrandPrix"
             )]
        public void FindFirstConstructorArgumentOfTypeNegativeCase()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GrandPrix));
            plugin.FindFirstConstructorArgumentOfType<IWidget>();
        }

        [Test]
        public void GetFirstMarkedConstructor()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ComplexRule));
            ConstructorInfo constructor = plugin.GetConstructor();

            Assert.IsNotNull(constructor);
            Assert.AreEqual(7, constructor.GetParameters().Length, "Should have 7 inputs, not 8");
        }

        [Test]
        public void GetGreediestConstructor()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GreaterThanRule));
            ConstructorInfo constructor = plugin.GetConstructor();

            Assert.IsNotNull(constructor);
            Assert.AreEqual(2, constructor.GetParameters().Length, "Should have 2 inputs");
        }

        [Test]
        public void GetPluginsIncludingTheBaseClass()
        {
            Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
            PluginFamily family = new PluginFamily(typeof (GrandChild));
            Plugin[] plugs = family.FindPlugins(new AssemblyGraph(assem));


            Assert.IsNotNull(plugs);
            Assert.AreEqual(2, plugs.Length);
        }

        [Test]
        public void GetPluginsOfAnAbstractClass()
        {
            Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
            PluginFamily family = new PluginFamily(typeof (WidgetMaker));
            Plugin[] plugs = family.FindPlugins(new AssemblyGraph(assem));

            Assert.IsNotNull(plugs);
            Assert.AreEqual(2, plugs.Length);
        }

        [Test]
        public void GetPluginsOfAnInterface()
        {
            Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
            PluginFamily family = new PluginFamily(typeof (IWidget));
            Plugin[] plugs = family.FindPlugins(new AssemblyGraph(assem));

            Assert.IsNotNull(plugs);
            Assert.AreEqual(4, plugs.Length);
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
                Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ClassWithNoConstructor));
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