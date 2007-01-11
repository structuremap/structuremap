using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
	[TestFixture]
	public class InstanceManagerTester
	{
		private InstanceManager _manager;
		private InstanceFactory _ruleFactory;
		private InstanceFactory _widgetFactory;
		private InstanceFactory _widgetMakerFactory;

		public InstanceManagerTester()
		{
		}


		[SetUp]
		public void SetUp()
		{
			string[] assemblyNames = new string[] {"StructureMap.Testing.Widget"};

			_ruleFactory = ObjectMother.CreateInstanceFactory(typeof (Rule), assemblyNames);
			_widgetFactory = ObjectMother.CreateInstanceFactory(typeof (IWidget), assemblyNames);
			_widgetMakerFactory =
				ObjectMother.CreateInstanceFactory(typeof (WidgetMaker), assemblyNames);

			_ruleFactory.Source = new MemoryMementoSource();
			_widgetFactory.Source = new MemoryMementoSource();
			_widgetMakerFactory.Source = new MemoryMementoSource(); 

			_manager = new InstanceManager();
			_manager.RegisterType(_ruleFactory);
			_manager.RegisterType(_widgetFactory);
			_manager.RegisterType(_widgetMakerFactory);
		}

		private void addColorMemento(string Color)
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("Color", Color);
			memento.SetProperty("Color", Color);

			((MemoryMementoSource) _ruleFactory.Source).AddMemento(memento);
			((MemoryMementoSource) _widgetFactory.Source).AddMemento(memento);
			((MemoryMementoSource) _widgetMakerFactory.Source).AddMemento(memento);
		}

		[Test, ExpectedException(typeof (StructureMapException))]
		public void GetMissingType()
		{
			object o = _manager.CreateInstance(typeof (string));
		}

		[Test]
		public void GetDefaultInstance()
		{
			addColorMemento("Red");
			addColorMemento("Orange");
			addColorMemento("Blue");

			_manager.SetDefault(typeof (Rule), "Blue");
			ColorRule rule = _manager.CreateInstance(typeof (Rule)) as ColorRule;

			Assert.IsNotNull(rule);
			Assert.AreEqual("Blue", rule.Color);
		}

		[Test]
		public void GetInstanceOf3Types()
		{
			addColorMemento("Red");
			addColorMemento("Orange");
			addColorMemento("Blue");

			ColorRule rule = _manager.CreateInstance(typeof (Rule), "Blue") as ColorRule;
			Assert.IsNotNull(rule);
			Assert.AreEqual("Blue", rule.Color);

			ColorWidget widget = _manager.CreateInstance(typeof (IWidget), "Red") as ColorWidget;
			Assert.IsNotNull(widget);
			Assert.AreEqual("Red", widget.Color);

			ColorWidgetMaker maker = _manager.CreateInstance(typeof (WidgetMaker), "Orange") as ColorWidgetMaker;
			Assert.IsNotNull(maker);
			Assert.AreEqual("Orange", maker.Color);
		}

		[Test]
		public void CreatesInterceptionChainOnInstanceFactory()
		{
			// Create a PluginFamily with one Interceptor in the InterceptionChain of the Family
			PluginFamily family = new PluginFamily(typeof (Rule));
			SingletonInterceptor interceptor = new SingletonInterceptor();
			family.InterceptionChain.AddInterceptor(interceptor);

			// Create a PluginGraph with the PluginFamily
			PluginGraph pluginGraph = new PluginGraph();
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget");
			pluginGraph.PluginFamilies.Add(family);
			pluginGraph.Seal();

			// Create an InstanceManager and examine the InstanceFactory for the PluginFamily
			InstanceManager instanceManager = new InstanceManager(pluginGraph);

			IInstanceFactory wrappedFactory = instanceManager[typeof (Rule)];
			Assert.AreSame(interceptor, wrappedFactory);

			InstanceFactory factory = (InstanceFactory) interceptor.InnerInstanceFactory;
			Assert.AreEqual(typeof (Rule), factory.PluginType);
		}


	    [Test]
	    public void FindAPluginFamilyForAGenericTypeFromPluginTypeName()
	    {
	        Type serviceType = typeof (IService<>);
	        PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);

            InstanceManager manager = new InstanceManager(pluginGraph);

	        Type stringService = typeof (IService<string>);

	        IInstanceFactory factory = manager[stringService];
            Assert.AreEqual(stringService, factory.PluginType);
	    }

	}
}