using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class InstanceValidatorTester
	{
		[Test]
		public void InstanceExistsPositive()
		{
			PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
			InstanceManager manager = new InstanceManager(pluginGraph);
			InstanceValidator validator = new InstanceValidator(pluginGraph, new Profile("profile"), manager);
			Assert.IsTrue(validator.InstanceExists(typeof(Rule), "Red"));
		}

		[Test]
		public void InstanceExistsNegative()
		{
			PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
			InstanceManager manager = new InstanceManager(pluginGraph);
			InstanceValidator validator = new InstanceValidator(pluginGraph, new Profile("profile"), manager);
			Assert.IsFalse(validator.InstanceExists(typeof(IGateway), "SomethingWrong"));
		}
	}
}
