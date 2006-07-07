using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
	[TestFixture]
	public class GraphDeploymentTester
	{
		[Test]
		public void AssemblyGraphDeployment()
		{
			AssemblyGraph assembly1 = new AssemblyGraph("StructureMap.Testing.Widget");

			AssemblyGraph assembly2 = new AssemblyGraph("StructureMap.Testing.Widget2");
			assembly2.DeploymentTargets = new string[] {"Server"};

			AssemblyGraph assembly3 = new AssemblyGraph("StructureMap.Testing.Widget3");
			assembly3.DeploymentTargets = new string[] {"All", "Client"};


			// assembly1 is not limited for deployment, so is all
			Assert.IsTrue(assembly1.IsDeployed("Client"));
			Assert.IsTrue(assembly1.IsDeployed("Server"));

			// assembly2 is limited to "Server"
			Assert.IsFalse(assembly2.IsDeployed("Client"));
			Assert.IsTrue(assembly2.IsDeployed("Server"));

			// assembly1 is not limited for deployment, so is all
			Assert.IsTrue(assembly3.IsDeployed("Client"));
			Assert.IsTrue(assembly3.IsDeployed("Server"));
		}

		[Test]
		public void PluginFamilyDeployment()
		{
			PluginFamily family1 = new PluginFamily(typeof (IWidget));

			PluginFamily family2 = new PluginFamily(typeof (IWidget));
			family2.DeploymentTargets = new string[] {"Server"};

			PluginFamily family3 = new PluginFamily(typeof (IWidget));
			family3.DeploymentTargets = new string[] {"All", "Client"};

			// family1 is not limited for deployment, so is all
			Assert.IsTrue(family1.IsDeployed("Client"));
			Assert.IsTrue(family1.IsDeployed("Server"));

			// family2 is limited to "Server"
			Assert.IsFalse(family2.IsDeployed("Client"));
			Assert.IsTrue(family2.IsDeployed("Server"));

			// family1 is not limited for deployment, so is all
			Assert.IsTrue(family3.IsDeployed("Client"));
			Assert.IsTrue(family3.IsDeployed("Server"));
		}


	}
}