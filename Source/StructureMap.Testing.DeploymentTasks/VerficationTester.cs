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
		    PluginGraphReport report = PluginGraphBuilder.BuildReportFromXml("Invalid.xml");
		    ProblemFinder finder = new ProblemFinder(report);
			Problem[] problems = finder.GetProblems();

			Assert.IsTrue(problems.Length > 0);
		}


	}
}