using System.IO;
using NUnit.Framework;
using StructureMap.DeploymentTasks.Versioning;

namespace StructureMap.Testing.Versioning
{
    [TestFixture]
    public class DeployedDirectoryTester
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            DirectoryInfo dir = new DirectoryInfo(@".");
            DeployedDirectory deployedDirectory = new DeployedDirectory(dir);

            deployedDirectory.WriteToXml("manifest.xml");

            DeployedDirectory directory2 = DeployedDirectory.ReadFromXml("manifest.xml");

            Assert.AreEqual(deployedDirectory.Assemblies.Length, directory2.Assemblies.Length);
            Assert.AreEqual(deployedDirectory.DeployedFiles.Length, directory2.DeployedFiles.Length);
        }
    }
}