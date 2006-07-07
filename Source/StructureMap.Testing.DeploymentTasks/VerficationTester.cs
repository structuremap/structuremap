using NUnit.Framework;
using StructureMap.Configuration;

namespace StructureMap.Testing.DeploymentTasks
{
	[TestFixture]
	public class VerficationTester
	{
		[Test]
		public void ExpectFailureWithinProblemFinder()
		{
			PluginGraphBuilder builder = new PluginGraphBuilder("Invalid.xml");
			ProblemFinder finder = new ProblemFinder(builder.Report);
			Problem[] problems = finder.GetProblems();

			Assert.IsTrue(problems.Length > 0);
		}


	}
}