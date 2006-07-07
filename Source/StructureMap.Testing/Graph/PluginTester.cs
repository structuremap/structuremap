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
		private Plugin _plugin;
		private Type _iwidget;
		private Type _widgetmaker;
		private Type _colorwidget;
		private Type _moneywidgetmaker;

		public PluginTester()
		{
		}


		[SetUp]
		public void SetUp()
		{
			_plugin = Plugin.CreateImplicitPlugin(typeof (ConfigurationWidget));
			_iwidget = typeof (IWidget);
			_widgetmaker = typeof (WidgetMaker);
			_colorwidget = typeof (ColorWidget);
			_moneywidgetmaker = typeof (MoneyWidgetMaker);
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
		public void CreateImplicitPluginDefinitionSourceIsImplicity()
		{
			Assert.AreEqual(DefinitionSource.Implicit, _plugin.DefinitionSource);
		}


		[Test, ExpectedException(typeof (ApplicationException))]
		public void NotAPluggableClass()
		{
			_plugin = Plugin.CreateImplicitPlugin(typeof (NotPluggable));
		}


		[Test]
		public void ValidationMethods()
		{
			MemberInfo[] methods = _plugin.ValidationMethods;
			Assert.IsNotNull(methods);
			Assert.AreEqual(methods.Length, 2);
		}


		[Test]
		public void CanPluginWithAttribute()
		{
			Assert.AreEqual(true, Plugin.CanBePluggedIn(_iwidget, _colorwidget), "ColorWidget plugs into IWidget");
		}

		[Test]
		public void CanNotPluginWithoutAttribute()
		{
			string msg = "NotPluggableWidget cannot plug into IWidget automatically";
			Assert.AreEqual(false, Plugin.CanBePluggedIn(_iwidget, typeof (NotPluggable)), msg);
		}


		[Test]
		public void GetPluginsOfAnInterface()
		{
			Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
			Plugin[] plugs = Plugin.GetPlugins(assem, typeof (IWidget));

			Assert.IsNotNull(plugs);
			Assert.AreEqual(3, plugs.Length);
		}


		[Test]
		public void GetPluginsOfAnAbstractClass()
		{
			Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
			Plugin[] plugs = Plugin.GetPlugins(assem, typeof (WidgetMaker));

			Assert.IsNotNull(plugs);
			Assert.AreEqual(2, plugs.Length);
		}

		[Test]
		public void GetPluginsIncludingTheBaseClass()
		{
			Assembly assem = AppDomain.CurrentDomain.Load("StructureMap.Testing.Widget");
			Plugin[] plugs = Plugin.GetPlugins(assem, typeof (GrandChild));

			Assert.IsNotNull(plugs);
			Assert.AreEqual(2, plugs.Length);
		}


		[Test]
		public void GoodPluginToInterface()
		{
			Assert.AreEqual(true, Plugin.CanBeCast(_iwidget, _colorwidget), "ColorWidget is an IWidget");
		}


		[Test]
		public void BadPluginToInterface()
		{
			Assert.AreEqual(false, Plugin.CanBeCast(_iwidget, _moneywidgetmaker), "MoneyWidgetMaker is NOT an IWidget");
		}

		[Test]
		public void GoodPluginToAbstractClass()
		{
			Assert.AreEqual(true, Plugin.CanBeCast(_widgetmaker, _moneywidgetmaker), "MoneyWidgetMaker is a WidgetMaker");
		}

		[Test]
		public void BadPluginToAbstractClass()
		{
			Assert.AreEqual(false, Plugin.CanBeCast(_widgetmaker, _colorwidget), "ColorWidget is NOT a WidgetMaker");
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
		public void GetFirstMarkedConstructor()
		{
			Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ComplexRule));
			ConstructorInfo constructor = plugin.GetConstructor();

			Assert.IsNotNull(constructor);
			Assert.AreEqual(7, constructor.GetParameters().Length, "Should have 7 inputs, not 8");
		}

		[Test]
		public void CanNotMakeObjectInstanceActivator()
		{
			Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ComplexRule));
			Assert.IsTrue(plugin.HasConstructorArguments(), "ComplexRule cannot be just an activator");
		}

		[Test]
		public void CanMakeObjectInstanceActivator()
		{
			Plugin plugin = Plugin.CreateImplicitPlugin(typeof (DefaultGateway));
			Assert.IsTrue(!plugin.HasConstructorArguments(), "DefaultGateway can be just an activator");
		}


		[Test, ExpectedException(typeof (StructureMapException), "StructureMap Exception Code:  112\nMissing a mandatory \"ConcreteKey\" attribute in a <Plugin> node for Type \"StructureMap.Testing.Widget.NotPluggableWidget\"")]
		public void AddAPluggedTypeWithoutAConcreteKey()
		{
			TypePath path = new TypePath("StructureMap.Testing.Widget",
			                             "StructureMap.Testing.Widget.NotPluggableWidget");

			Plugin plugin = new Plugin(path, "");
		}


		[Test, ExpectedException(typeof (StructureMapException), "StructureMap Exception Code:  110\nAssembly referenced by a <Plugin> for Type \"IDontExist.Assembly.NotPluggableWidget\", concrete key \"default\" node in StructureMap.config cannot be loaded into the current AppDomain.")]
		public void CreateAPluginWithANonExistentAssembly()
		{
			TypePath path = new TypePath("IDontExist.Assembly",
			                             "IDontExist.Assembly.NotPluggableWidget");

			Plugin plugin = new Plugin(path, "default");
		}


		[Test, ExpectedException(typeof (StructureMapException), "StructureMap Exception Code:  111\nType referenced in a <Plugin> for Type \"StructureMap.Testing.Widget.NonExistentClass\", concrete type \"default\" cannot be loaded from named assembly")]
		public void CreateAPluginWithANonExistentClass()
		{
			TypePath path = new TypePath("StructureMap.Testing.Widget",
			                             "StructureMap.Testing.Widget.NonExistentClass");

			Plugin plugin = new Plugin(path, "default");
		}



		[Test]
		public void CreateImplicitMementoWithSomeConstructorArgumentsReturnValueIsNull()
		{
			Plugin plugin = Plugin.CreateExplicitPlugin(typeof (Strategy), "Default", string.Empty);
			InstanceMemento memento = plugin.CreateImplicitMemento();
			Assert.IsNull(memento);
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
		public void CanBeAutoFilledIsTrue()
		{
			Plugin plugin = Plugin.CreateImplicitPlugin(typeof (Mustang));

			Assert.IsTrue(plugin.CanBeAutoFilled);
		}

		[Test]
		public void CanBeAutoFilledIsFalse()
		{
			Plugin plugin = Plugin.CreateImplicitPlugin(typeof (GrandPrix));

			Assert.IsFalse(plugin.CanBeAutoFilled);
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
		private readonly IEngine _engine;
		private readonly string _color;
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


}