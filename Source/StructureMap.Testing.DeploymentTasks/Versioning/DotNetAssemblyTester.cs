using NMock;
using NUnit.Framework;
using StructureMap.DeploymentTasks.Versioning;

namespace StructureMap.Testing.Versioning
{
	[TestFixture]
	public class DotNetAssemblyTester
	{
		[Test]
		public void CheckVersionIsPerfectlyFine()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DotNetAssembly assembly = new DotNetAssembly("some assembly", "1.0.0.0");

			DeployedDirectory directory = new DeployedDirectory();
			directory.AddAssembly(assembly);

			assembly.CheckVersion(directory, report);

			versionMock.Verify();
		}


		[Test]
		public void CheckVersionFindsAMissingAssembly()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DotNetAssembly assembly = new DotNetAssembly("some assembly", "1.0.0.0");

			DeployedDirectory directory = new DeployedDirectory();

			versionMock.Expect("MissingAssembly", assembly.AssemblyName, assembly.Version);

			assembly.CheckVersion(directory, report);

			versionMock.Verify();
		}


		[Test]
		public void CheckVersionFindsAVersionConflict()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DotNetAssembly assembly = new DotNetAssembly("some assembly", "1.0.0.0");

			DeployedDirectory directory = new DeployedDirectory();
			DotNetAssembly assembly2 = new DotNetAssembly(assembly.AssemblyName, "1.0.0.1");
			directory.AddAssembly(assembly2);

			versionMock.Expect("VersionMismatchAssembly", assembly.AssemblyName, assembly.Version, assembly2.Version);

			assembly.CheckVersion(directory, report);

			versionMock.Verify();
		}
	}
}
