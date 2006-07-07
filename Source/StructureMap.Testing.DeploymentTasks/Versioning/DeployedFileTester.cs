using NMock;
using NUnit.Framework;
using StructureMap.DeploymentTasks.Versioning;

namespace StructureMap.Testing.Versioning
{
	[TestFixture]
	public class DeployedFileTester
	{
		[Test]
		public void CheckVersionFindsTheFile()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DeployedFile file = new DeployedFile("some file");
			file.ContentsHash = new byte[0];

			DeployedDirectory directory = new DeployedDirectory();
			directory.AddFile(file);

			file.CheckVersion(directory, report);

			versionMock.Verify();
		}

		[Test]
		public void CheckVersionFindsAMissingFile()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DeployedFile file = new DeployedFile("some file");
			file.ContentsHash = new byte[0];

			DeployedDirectory directory = new DeployedDirectory();

			versionMock.Expect("MissingFile", file.FileName);

			file.CheckVersion(directory, report);

			versionMock.Verify();
		}


		[Test]
		public void CheckVersionFindsAVersionConflict()
		{
			DynamicMock versionMock = new DynamicMock(typeof(IVersionReport));
			IVersionReport report = (IVersionReport) versionMock.MockInstance;

			DeployedFile file = new DeployedFile("some file");
			file.ContentsHash = new byte[0];

			DeployedDirectory directory = new DeployedDirectory();

			DeployedFile otherFile = new DeployedFile(file.FileName);
			otherFile.ContentsHash = new byte[]{1,1,2};
			directory.AddFile(otherFile);

			versionMock.Expect("VersionMismatchFile", file.FileName);

			file.CheckVersion(directory, report);

			versionMock.Verify();
		}
	}
}
