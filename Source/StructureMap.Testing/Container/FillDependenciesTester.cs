using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Container
{
	[TestFixture]
	public class FillDependenciesTester
	{
		private PluginFamily _family;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_family = PluginFamily.CreateAutoFilledPluginFamily(typeof (FilledConcreteClass));
			Assert.IsNotNull(_family);
		}

		[Test]
		public void CreateAutoFilledPluginFamily()
		{
			Assert.IsNotNull(_family);
			Assert.AreEqual(typeof (FilledConcreteClass), _family.PluginType);
			Assert.AreEqual(DefinitionSource.Implicit, _family.DefinitionSource);
			Assert.AreEqual(PluginFamily.CONCRETE_KEY, _family.DefaultInstanceKey);
		}

		[Test]
		public void CreatesMementoSource()
		{
			MemoryMementoSource source = (MemoryMementoSource) _family.Source;
		}

		[Test]
		public void CreatesPlugin()
		{
			Assert.AreEqual(1, _family.Plugins.Count);
			Plugin plugin = _family.Plugins[PluginFamily.CONCRETE_KEY];
			Assert.IsNotNull(plugin);

			Assert.AreEqual(PluginFamily.CONCRETE_KEY, plugin.ConcreteKey);
			Assert.AreEqual(typeof (FilledConcreteClass), plugin.PluggedType);
		}

		[Test]
		public void CanFillDependenciesSuccessfully()
		{
			PluginGraph pluginGraph = ObjectMother.GetPluginGraph();

			InstanceManager manager = new InstanceManager(pluginGraph);

			// The dependencies must have a default setting first
			manager.SetDefault(typeof (IStrategy), "Red");
			manager.SetDefault(typeof (IWidget), "Blue");
			IWidget widget = (IWidget) manager.CreateInstance(typeof (IWidget));
			IStrategy strategy = (IStrategy) manager.CreateInstance(typeof (IStrategy));

			FilledConcreteClass concreteClass =
				(FilledConcreteClass) manager.FillDependencies(typeof (FilledConcreteClass));

			Assert.IsNotNull(concreteClass.Widget);
			Assert.IsNotNull(concreteClass.Strategy);
		}

		[Test, ExpectedException(typeof (StructureMapException))]
		public void TryToFillDependenciesOnAbstractClassThrowsException()
		{
			PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
			InstanceManager manager = new InstanceManager(pluginGraph);

			manager.FillDependencies(typeof (AbstractClass));
		}


		[Test, ExpectedException(typeof (StructureMapException))]
		public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
		{
			PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
			InstanceManager manager = new InstanceManager(pluginGraph);

			manager.FillDependencies(typeof (CannotBeFilledConcreteClass));
		}
	}

	public class FilledConcreteClass
	{
		private readonly IWidget _widget;
		private readonly IStrategy _strategy;

		public FilledConcreteClass(IStrategy strategy, IWidget widget)
		{
			_strategy = strategy;
			_widget = widget;
		}

		public IStrategy Strategy
		{
			get { return _strategy; }
		}

		public IWidget Widget
		{
			get { return _widget; }
		}
	}

	public class CannotBeFilledConcreteClass
	{
		public CannotBeFilledConcreteClass(string name, Rule rule)
		{
		}
	}

	public abstract class AbstractClass
	{
	}
}